using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Application;
using Application.Features.Identity.Roles.Contracts;
using Application.Features.Identity.Tokens;
using Application.Features.Identity.Users.Contracts;
using Application.Features.Schools;
using Application.Features.Tenancy;
using Application.Wrappers;
using Finbuckle.MultiTenant;
using Infrastructure.Constants;
using Infrastructure.Contexts;
using Infrastructure.Identity;
using Infrastructure.Identity.Auth;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Tokens;
using Infrastructure.OpenApi;
using Infrastructure.Schools;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add your infrastructure services here,
            // For example, database context, repositories, etc.
            // Example:
            return services
                .AddDbContext<TenantDbContext>(options => options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                 .AddMultiTenant<ABCSchoolTenantInfo>()
                 .WithHeaderStrategy(TenancyConstants.TenantIdName)
                 .WithClaimStrategy(TenancyConstants.TenantIdName)
                 .WithEFCoreStore<TenantDbContext, ABCSchoolTenantInfo>().Services
                 .AddDbContext<ApplicationDbContext>(options =>
                     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                 
                 .AddTransient<ITenantDbSeeder, TenantDbSeeder>()
                 .AddTransient<ApplicationDbSeeder>()
                .AddTransient<ITenantService,TenantService>()
                .AddTransient<ISchoolService,SchoolService>()
                 .AddIdentityService()
                .AddPermissions()
                .AddOpenApiDocumentation(configuration);
        }

        public static async Task AddDatabaseInitializerAsync(this IServiceProvider serviceProvider,CancellationToken ct=default)
        {
            using var scope = serviceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<ITenantDbSeeder>().InitializeDatabaseAsync(ct);
        }

        private static IServiceCollection AddIdentityService(this IServiceCollection services)
        {
            return services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders().Services
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IRoleService, RoleService>();
        }

        private static IServiceCollection AddPermissions(this IServiceCollection services)
        {
            return services
                .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }

        public static JwtSettings GetJwtSettings(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            var jwtSettingsConfig = configuration.GetSection(nameof(JwtSettings));
            serviceCollection.Configure<JwtSettings>(jwtSettingsConfig);
            return jwtSettingsConfig.Get<JwtSettings>()!;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection service, JwtSettings jwtSettings)
        {
            var secret = Encoding.ASCII.GetBytes(jwtSettings.Secret);

            service.AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                    };

                    bearer.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.StatusCode =(int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                var result =
                                    JsonConvert.SerializeObject(ResponseWrapper.Fail("Token has expired"));
                                return context.Response.WriteAsync(result);
                            }
                            else
                            {
                                if (!context.Response.HasStarted)
                                {
                                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                    context.Response.ContentType = "application/json";
                                    var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("An unhandled error has occured."));
                                    return context.Response.WriteAsync(result);
                                }
                                return Task.CompletedTask;
                            }
                        },
                        OnChallenge = context =>
                        {
                            // Skip the default logic and handle the response manually
                            context.HandleResponse();
                            if (context.Response.HasStarted) return Task.CompletedTask;
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized."));
                            // Debug.WriteLine(result);
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";
                            var result =   JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access this resource."));
                            return context.Response.WriteAsync(result);
                        }
                    };
                });

            service.AddAuthorization(opt =>
            {
                foreach (var prop in typeof(SchoolPermissions).GetNestedTypes()
                             .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static |
                                                                BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                    {
                        opt.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(
                            ClaimConstants.Permission, propertyValue.ToString()));
                    }
                }
            });
            return service;
        }

        internal static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, IConfiguration config)
        {
            var swaggerSettings = config.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();

            services.AddEndpointsApiExplorer();
            _ = services.AddOpenApiDocument((document, serviceProvider) =>
            {
                document.PostProcess = doc =>
                {
                    doc.Info.Title = swaggerSettings.Title;
                    doc.Info.Description = swaggerSettings.Description;
                    doc.Info.Contact = new OpenApiContact
                    {
                        Name = swaggerSettings.ContactName,
                        Email = swaggerSettings.ContactEmail,
                        Url = swaggerSettings.ContactUrl
                    };
                    doc.Info.License = new OpenApiLicense
                    {
                        Name = swaggerSettings.LicenseName,
                        Url = swaggerSettings.LicenseUrl
                    };
                };

                document.AddSecurity(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter your Bearer token to attach it as a header on your requests.",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                });
                
                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
                document.OperationProcessors.Add(new SwaggerGlobalAuthProcessor());
                document.OperationProcessors.Add(new SwaggerHeaderAttributeProcessor());
            });

            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            // Configure your middleware here,
            // For example, authentication, logging, etc.
            // Example:
            app.UseAuthentication()
                .UseMultiTenant()
                .UseAuthorization()
                .UseOpenApiDocumentation();
            return app;
        }

        private static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app)
        {
            app.UseOpenApi();
            app.UseSwaggerUi(opt =>
            {
                opt.DefaultModelExpandDepth = -1;
                opt.DocExpansion = "none";
                opt.TagsSorter = "alpha";
            });
            return app;
        }
    }
}

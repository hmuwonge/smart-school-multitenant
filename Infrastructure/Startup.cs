using Finbuckle.MultiTenant;
using Infrastructure.Contexts;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add your infrastructure services here
            // For example, database context, repositories, etc.
            // Example:
            return services.AddDbContext<TenantDbContext>(options =>
                  options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                 .AddMultiTenant<ABCSchoolTenantInfo>()
                 .WithHeaderStrategy(TenancyConstants.TenantIdName)
                 .WithClaimStrategy(TenancyConstants.TenantIdName)
                 .WithEFCoreStore<TenantDbContext, ABCSchoolTenantInfo>().Services
                 .AddDbContext<ApplicationDbContext>(options =>
                     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                 
                 .AddTransient<ITenantDbSeeder, TenantDbSeeder>()
                 .AddTransient<ApplicationDbSeeder>()
                 .AddIdentityService();

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
            .AddDefaultTokenProviders().Services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            // Configure your middleware here
            // For example, authentication, logging, etc.
            // Example:
            app.UseMultiTenant();
            return app;
        }
    }
}

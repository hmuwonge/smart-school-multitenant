using Finbuckle.MultiTenant;
using Infrastructure.Contexts;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Builder;
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
                     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

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

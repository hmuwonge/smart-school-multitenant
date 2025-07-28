using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tenancy
{
    public class TenantDbSeeder : ITenantDbSeeder
    {
        private readonly TenantDbContext _tenantDbContext;
        private readonly IServiceProvider _serviceProvider;

        public TenantDbSeeder(TenantDbContext tenantDbContext, IServiceProvider serviceProvider)
        {
            _tenantDbContext = tenantDbContext;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Method to initialize databases
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task InitializeDatabaseAsync(CancellationToken ct)
        {
            //seed tenant data
            await InitializeDatabaseWithTenantAsync(ct);

            foreach (var tenant in await _tenantDbContext.TenantInfo.ToListAsync(ct))
            {
                // application deb seeder
                await InitializeApplicationDbForTenantAsync(tenant,ct);
            }
        }


        /// <summary>
        /// Method that initializes database with tenancy
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task InitializeDatabaseWithTenantAsync(CancellationToken ct)
        {
            if(await _tenantDbContext.TenantInfo.FindAsync([TenancyConstants.Root.Id],ct) is null )
            {
                //create tenant
                var rootTenant = new ABCSchoolTenantInfo
                {
                    Id = TenancyConstants.Root.Id,
                    Identifier = TenancyConstants.Root.Id,
                    Name = TenancyConstants.Root.Name,
                    Email = TenancyConstants.Root.Email,
                    FirstName = TenancyConstants.FirstName,
                    LastName = TenancyConstants.LastName,
                    IsActive = true,
                    ValidUpTo = DateTime.UtcNow.AddYears(2)
                };

                await _tenantDbContext.TenantInfo.AddAsync(rootTenant,ct);
                await _tenantDbContext.SaveChangesAsync(ct);
            }
        }

        private async Task InitializeApplicationDbForTenantAsync(ABCSchoolTenantInfo tenant,CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();

            _serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
                .MultiTenantContext = new MultiTenantContext<ABCSchoolTenantInfo>()
                {
                    TenantInfo = tenant,
                };

            await scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>().InitializeDatabaseAsync(ct);
        }
    }

    
}

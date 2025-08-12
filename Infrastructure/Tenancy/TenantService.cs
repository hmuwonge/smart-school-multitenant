using Application.Features.Tenancy;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Contexts;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tenancy;

public class TenantService : ITenantService
{
    private readonly IMultiTenantStore<ABCSchoolTenantInfo> _tenantStore;
    private readonly ApplicationDbSeeder _dbSeeder;
    private readonly IServiceProvider _serviceProvider;

    public TenantService(IMultiTenantStore<ABCSchoolTenantInfo> tenantStore, ApplicationDbSeeder dbSeeder, IServiceProvider serviceProvider)
    {
        _tenantStore = tenantStore;
        _dbSeeder = dbSeeder;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> CreateTenantAsync(CreateTenantRequest request, CancellationToken ct)
    {
        var newTenant = new ABCSchoolTenantInfo
        {
            Id = request.Identifier,
            Identifier = request.Identifier,
            Name = request.Name,
            IsActive = request.IsActive,
            ConnectionString = request.ConnectionString,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            ValidUpTo = request.ValidUpTo
        };

        await _tenantStore.TryAddAsync(newTenant);
        
        // seeding tenant data
        using var scope = _serviceProvider.CreateScope();
        
            _serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
            .MultiTenantContext = new MultiTenantContext<ABCSchoolTenantInfo>()
        {
            TenantInfo = newTenant
        };

        await scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>().InitializeDatabaseAsync(ct);
        return newTenant.Identifier;
    }

    public async Task<string> ActivateAsync(string tenantId)
    {
        var tenantInDb = await _tenantStore.TryGetAsync(tenantId);
        tenantInDb.IsActive = true;

        await _tenantStore.TryUpdateAsync(tenantInDb);
        return tenantInDb.Identifier;
    }

    public async Task<string> DeactivateAsync(string tenantId)
    {
        var tenantInDb = await _tenantStore.TryGetAsync(tenantId);
        tenantInDb.IsActive = false;

        await _tenantStore.TryUpdateAsync(tenantInDb);
        return tenantInDb.Identifier;
    }

    public async Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscriptionRequest)
    {
        var tenantInDb = await _tenantStore.TryGetAsync(updateTenantSubscriptionRequest.TenantId);
        tenantInDb.ValidUpTo = updateTenantSubscriptionRequest.NewExpiryDate;
        await _tenantStore.TryUpdateAsync(tenantInDb);
        return tenantInDb.Identifier;
    }

    public async Task<List<TenantResponse>> GetTenantsAsync()
    {
        var tenantInDb = await _tenantStore.GetAllAsync();
        return tenantInDb.Adapt<List<TenantResponse>>();
    }

    public async Task<TenantResponse> GetTenantByIdAsync(string tenantId)
    {
        var tenantInDb = await _tenantStore.TryGetAsync(tenantId);

        #region Manual Mapping

        

        #endregion

        return tenantInDb.Adapt<TenantResponse>();
    }
}
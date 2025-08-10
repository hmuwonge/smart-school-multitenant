
namespace Application.Features.Tenancy
{
    public interface ITenantService
    {
        Task<string> CreateTenantAsync(CreateTenantRequest request, CancellationToken ct);
        Task<string> ActivateAsync(string tenantId);
        Task<string> DeactivateAsync(string tenantId);
        Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscriptionRequest);
        Task<List<TenantResponse>> GetTenantsAsync();
        Task<TenantResponse> GetTenantByIdAsync(string tenantId);
    }
}

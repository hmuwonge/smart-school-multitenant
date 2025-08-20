namespace Application.Features.Identity.Roles.Contracts;

public interface IRoleService
{
    Task<string> CreateAsync(CreateRoleRequest request);
    Task<string> UpdateAsync(UpdateRoleRequest request);
    Task<string> DeleteAsync(string id);
    Task<string> UpdatePermissionsAsync(UpdateRolePermissionRequest request);
    Task<bool> DoesItExistAsync(string name);
    Task<List<RoleResponse>> GetAllAsync(CancellationToken ct);
    Task<RoleResponse> GetByIdAsync(string id, CancellationToken ct);
    Task<RoleResponse> GetRoleWthPermissionsAsync(string id, CancellationToken ct);
}
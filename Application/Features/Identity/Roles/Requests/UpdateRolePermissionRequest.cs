namespace Application.Features.Identity.Roles;

public class UpdateRolePermissionRequest
{
    public string RoleId { get; set; }
    public List<string> NewPermissions { get; set; } = [];
}
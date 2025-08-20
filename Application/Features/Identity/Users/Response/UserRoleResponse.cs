namespace Application.Features.Identity.Users.Response;

public class UserRoleResponse
{
    public string RoleId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsAssigned { get; set; }
}

public class UserRolesRequest
{
    public List<UserRoleResponse> UserRoles { get; set; } = [];
}
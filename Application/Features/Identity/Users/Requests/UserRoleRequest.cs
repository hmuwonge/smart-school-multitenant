namespace Application.Features.Identity.Users.Requests;

public class UserRoleRequest
{
    public string RoleId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsAssigned { get; set; }
}

public class UserRolesRequest
{
    public List<UserRoleRequest> UserRoles { get; set; } = [];
}
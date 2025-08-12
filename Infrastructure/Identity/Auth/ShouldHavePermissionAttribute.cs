using Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Identity.Auth;

public class ShouldHavePermissionAttribute:AuthorizeAttribute
{
    public ShouldHavePermissionAttribute(string action, string feature)
    {
        Policy = SchoolPermission.NameFor(feature, action);
    }
}
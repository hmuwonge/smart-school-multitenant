using System.Security.Claims;
using Application.Exceptions;
using Application.Features.Identity.Users.Contracts;

namespace Infrastructure.Identity;

public class CurrentUserService:ICurrentUserService
{
    private ClaimsPrincipal _principal;
    // private ICurrentUserService _currentUserServiceImplementation;
    public string Name => _principal.Identity.Name;

    public string GetUserId()
    {
        return IsAuthenticated() ? _principal.GetUserId() : string.Empty;
    }

    public string GetUserTenant()
    {
        return IsAuthenticated() ? _principal.GetTenant() : string.Empty;
    }

    public string GetUserEmail()
    {
        return IsAuthenticated() ? _principal.GetEmail() : string.Empty;
    }

    public bool IsAuthenticated()
    {
        return _principal.Identity is { IsAuthenticated: true };
    }

    public bool IsInRole(string roleName)
    {
        return _principal.IsInRole(roleName);
    }

    public IEnumerable<Claim> GetUserClaims()
    {
        return _principal.Claims;
    }

    public void SetCurrentUser(ClaimsPrincipal principal)
    {
        if (_principal is not null)
        {
            throw new ConflictException(["Invalid operation on claim."]);
        }

        _principal = principal;
    }
}
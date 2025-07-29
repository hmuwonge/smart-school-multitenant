using Application.Exceptions;
using Application.Features.Identity.Tokens;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor;

    public TokenService(UserManager<ApplicationUser> userManager,
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor)
    {
        _userManager = userManager;
        _tenantContextAccessor = tenantContextAccessor;
    }
    public async Task<TokenResponse> LoginAsync(TokenRequest request)
    {
        if (!_tenantContextAccessor.MultiTenantContext.TenantInfo.IsActive)
        {
            throw new UnauthorizedException(["Tenant subscription is not active. Contact Admin"]);
        }

        var userInDb = await _userManager.FindByNameAsync(request.Username) ?? throw new UnauthorizedException([
        "Authentication not successful"]);

        if (!await _userManager.CheckPasswordAsync(userInDb, request.Password))
        {
            throw new UnauthorizedException(["Incorrect Username or Password"]);
        }

        if (!userInDb.IsActive)
        {
            throw new UnauthorizedException(["User Not Active.Contact Admin"]);
        }

        if (_tenantContextAccessor.MultiTenantContext.TenantInfo.Id is not TenancyConstants.Root.Id)
        {
            if (_tenantContextAccessor.MultiTenantContext.TenantInfo.ValidUpTo < DateTime.UtcNow)
            {
                throw new UnauthorizedException(["Tenant Subscription has expired. Contact Admin"]);
            }
        }
    }

    public Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        throw new NotImplementedException();
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application;
using Application.Exceptions;
using Application.Features.Identity.Tokens;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity.Tokens;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor;
    private readonly JwtSettings _jwtSettings;

    public TokenService(
        UserManager<ApplicationUser> userManager,
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor,
        RoleManager<ApplicationRole> roleManager,
        IOptions<JwtSettings> jwtSettings
        )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
        _tenantContextAccessor = tenantContextAccessor;
    }

    
    public async Task<TokenResponse> LoginAsync(TokenRequest request)
    {
        #region Validations
        if (!_tenantContextAccessor.MultiTenantContext.TenantInfo.IsActive)
        {
            throw new UnauthorizedException(HttpStatusCode.Unauthorized, ["Tenant subscription is not active. Contact Admin"]);
        }

        var userInDb = await _userManager.FindByNameAsync(request.Username) ?? throw new UnauthorizedException(HttpStatusCode.Unauthorized, [
        "Authentication not successful"]);

        if (!await _userManager.CheckPasswordAsync(userInDb, request.Password))
        {
            throw new UnauthorizedException(HttpStatusCode.Unauthorized, ["Incorrect Username or Password"]);
        }

        if (!userInDb.IsActive)
        {
            throw new UnauthorizedException(HttpStatusCode.Unauthorized, ["User Not Active.Contact Admin"]);
        }

        if (_tenantContextAccessor.MultiTenantContext.TenantInfo.Id is not TenancyConstants.Root.Id)
        {
            if (_tenantContextAccessor.MultiTenantContext.TenantInfo.ValidUpTo < DateTime.UtcNow)
            {
                throw new UnauthorizedException(HttpStatusCode.Unauthorized, ["Tenant Subscription has expired. Contact Admin"]);
            }
        }
        #endregion

        return await GenerateTokenAndUpdateUserAsync(userInDb);
    }

    private ClaimsPrincipal GetClaimsPrincipleFromExpiringToken(string expiringToken)
    {
        var tkValidationProcess = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(expiringToken, tkValidationProcess, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken 
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new UnauthorizedException(HttpStatusCode.Unauthorized, ["Invalid token provided. Failed to generate new token"]);
        }

        return principal;
    }

    public async Task<TokenResponse> GenerateTokenAndUpdateUserAsync(ApplicationUser user)
    {
        //generate jwt
        var newJwt = await GenerateToken(user);
        
        //refresh token
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryTimeInDays);

        await _userManager.UpdateAsync(user);

        return new TokenResponse
        {
            Jwt = newJwt,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryDate = user.RefreshTokenExpiryTime
        };
    }

    private async Task<string> GenerateToken(ApplicationUser user)
    {
        return GenerateEncryptedToken(GenerateSigningCredentials(),await GetUserClaims(user));
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: signingCredentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private SigningCredentials GenerateSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    private  async Task<IEnumerable<Claim>> GetUserClaims(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();
        var permissionClaims  = new List<Claim>();

        foreach (var userRole in userRoles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role,userRole));
            var currentRole = await _roleManager.FindByNameAsync(userRole);

            var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
            
            permissionClaims.AddRange(allPermissionsForCurrentRole);
        }
        
        var claims = new List<Claim>
            {
        new(ClaimTypes.NameIdentifier,user.Id),
        new(ClaimTypes.Email,user.Email ?? String.Empty),
        new(ClaimTypes.Name,user.FirstName),
        new(ClaimTypes.Surname,user.LastName),
        new(ClaimConstants.Tenant,_tenantContextAccessor.MultiTenantContext.TenantInfo.Id),
        new(ClaimTypes.MobilePhone,user.PhoneNumber ??  String.Empty),
        }.Union(roleClaims).Union(userClaims).Union(permissionClaims);

        return claims;
    }

    public  async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var userPrincipal = GetClaimsPrincipleFromExpiringToken(request.CurrentJwt);
        var userEmail = userPrincipal.GetEmail();

        var userInDb = await _userManager.FindByEmailAsync(userEmail) ??
                       throw new UnauthorizedException(HttpStatusCode.Unauthorized, ["Authorization failed"]);

        if (userInDb.RefreshToken != request.CurrentRefreshToken || userInDb.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            throw new UnauthorizedException(HttpStatusCode.Unauthorized, ["Invalid token."]);
        }

        return await GenerateTokenAndUpdateUserAsync(userInDb);
    }

    private string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }
}
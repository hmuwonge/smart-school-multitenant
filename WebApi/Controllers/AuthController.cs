using Application.Features.Identity.Tokens;
using Application.Features.Identity.Tokens.Queries;
using Infrastructure.Constants;
using Infrastructure.Identity.Auth;
using Infrastructure.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class AuthController:BaseApiController
{
    [HttpPost("Login")]
    [AllowAnonymous]
    [TenantHeader]
    [OpenApiOperation("Used to obtain jwt for login.")]
    public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest tokenRequest)
    {
        var response = await Sender.Send(new GetTokenQuery
        {
            TokenRequest = tokenRequest
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPost("refresh-token")]
    [OpenApiOperation("Used to generate new jwt from refresh token.")]
    [ShouldHavePermission(action: SchoolAction.RefreshToken,feature:SchoolFeature.Tokens)]
    public async Task<IActionResult> GetRefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var response = await Sender.Send(new GetRefreshTokenQuery
        {
            RefreshToken = refreshTokenRequest
        });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    [HttpGet("debug-claims")]
    [Authorize]
    [OpenApiOperation("Debug endpoint to check current user claims.")]
    public IActionResult GetCurrentUserClaims()
    {
        var claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();
        return Ok(new { Claims = claims });
    }
}
using Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Identity.Auth;

public class PermissionAuthorizationHandler:AuthorizationHandler<PermissionRequirement>
{
    private readonly ILogger<PermissionAuthorizationHandler> _logger;
    
    public PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger)
    {
        _logger = logger;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // Log all claims for debugging
        _logger.LogInformation("Checking permission: {Permission}", requirement.Permission);
        _logger.LogInformation("User claims: {Claims}", string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}")));
        
        var permissions = context.User.Claims.Where(claim => claim.Type == ClaimConstants.Permission
                                                             && claim.Value == requirement.Permission);

        if (permissions.Any())
        {
            _logger.LogInformation("Permission {Permission} found for user", requirement.Permission);
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("Permission {Permission} NOT found for user. Available permissions: {AvailablePermissions}", 
                requirement.Permission, 
                string.Join(", ", context.User.Claims.Where(c => c.Type == ClaimConstants.Permission).Select(c => c.Value)));
            context.Fail();
        }
        
        await Task.CompletedTask;
    }
}

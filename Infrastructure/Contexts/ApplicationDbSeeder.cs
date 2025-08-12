using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class ApplicationDbSeeder(
    IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor,
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext applicationDbContext)
{
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo>
        _tenantInfoContextAccessor = tenantInfoContextAccessor;

    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;


    /// <summary>
    /// Database initialization method
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        if (_applicationDbContext.Database.GetMigrations().Any())
        {
            if ((await _applicationDbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
            {
                await _applicationDbContext.Database.MigrateAsync(cancellationToken);
            }

            if (await _applicationDbContext.Database.CanConnectAsync(cancellationToken))
            {
                await InitializeDefaultRolesAsync(cancellationToken);
                // users
                await InitializeAdminUserAsync();                
            }
        }
    }

    /// <summary>
    /// Method that initializes default roles
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// 
    private async Task InitializeDefaultRolesAsync(CancellationToken ct)
    {
        foreach (var roleName in RoleConstants.DefaultRoles)
        {
            if (string.IsNullOrEmpty(roleName)) continue;

            // Modified query with null handling
            var existingRole = await _roleManager.Roles
                .Where(r => r.Name != null && r.Name == roleName)
                .SingleOrDefaultAsync(ct);

            if (existingRole is null)
            {
                var newRole = new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} Role",
                    // Initialize other required properties
                };

                var result = await _roleManager.CreateAsync(newRole);
                if (!result.Succeeded)
                {
                    // Log errors
                    continue;
                }
                existingRole = newRole;
            }

            // Assign permissions based on role
            if (roleName == RoleConstants.Admin)
            {
                await AssignPermissionsToRole(SchoolPermissions.Admin, existingRole, ct);

                if (_tenantInfoContextAccessor.MultiTenantContext?.TenantInfo?.Id == TenancyConstants.Root.Id)
                {
                    await AssignPermissionsToRole(SchoolPermissions.Root, existingRole, ct);
                }
            }
            else if (roleName == RoleConstants.Basic)
            {
                await AssignPermissionsToRole(SchoolPermissions.Basic, existingRole, ct);
            }
        }
    }
    private async Task InitializeDefaultRolesAsync2(CancellationToken ct)
    {
        foreach (var roleName in RoleConstants.DefaultRoles)
        {

            if (await _roleManager.Roles.SingleOrDefaultAsync(role => role.Name == roleName, ct) is not ApplicationRole incomingRole)
            {
                incomingRole = new ApplicationRole()
                {
                    Name = roleName,
                    Description = $"{roleName} Role",
                };
                await _roleManager.CreateAsync(incomingRole);
            }
            
            // assign prermissions
           
            if (roleName == RoleConstants.Admin)
            {
                // assign admin permissions
                await AssignPermissionsToRole(SchoolPermissions.Admin, incomingRole, ct);

                if (_tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Id == TenancyConstants.Root.Id)
                {
                    await AssignPermissionsToRole(SchoolPermissions.Root, incomingRole, ct);
                }
            }

            if (roleName == RoleConstants.Basic)
            {
                //assign basic permissions
                await AssignPermissionsToRole(SchoolPermissions.Basic, incomingRole, ct);
            }
        }
    }


    /// <summary>
    /// Method to assign permissions to roles
    /// </summary>
    /// <param name="rolePermissions"></param>
    /// <param name="role"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    private async Task AssignPermissionsToRole(IReadOnlyList<SchoolPermission> rolePermissions,
        ApplicationRole role, CancellationToken ct)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var rolePermission in rolePermissions)
        {
            if (!currentClaims.Any(c=>c.Type == ClaimConstants.Permission && c.Value == rolePermission.Name))
            {
                await _applicationDbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = rolePermission.Name,
                    Description = rolePermission.Description,
                    Group = rolePermission.Group
                },ct);
                await _applicationDbContext.SaveChangesAsync(ct);
            }
        }
    }


    /// <summary>
    /// Initialize admin users
    /// </summary>
    /// <returns></returns>
    private async Task InitializeAdminUserAsync()
    {
        if (string.IsNullOrEmpty(_tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email)) return;

        if(await _userManager.Users.SingleOrDefaultAsync(user=>user.Email == _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email) is not ApplicationUser incomingUser)
        {

            incomingUser = new ApplicationUser
            {
                FirstName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.FirstName,
                LastName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.LastName,
                Email = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email,
                UserName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email,
                EmailConfirmed =true,
                PhoneNumberConfirmed =true,
                NormalizedEmail = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email.ToUpperInvariant(),
                NormalizedUserName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email.ToUpperInvariant(),
                IsActive =true,
            };
            var passwordHash = new PasswordHasher<ApplicationUser>();
            incomingUser.PasswordHash = passwordHash.HashPassword(incomingUser, TenancyConstants.DefaultPassword);
            await _userManager.CreateAsync(incomingUser);
        }

        if(!await _userManager.IsInRoleAsync(incomingUser,RoleConstants.Admin))
        {
            await _userManager.AddToRoleAsync(incomingUser,RoleConstants.Admin);
        }
    }
}
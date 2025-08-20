using Application.Exceptions;
using Application.Features.Identity.Roles;
using Application.Features.Identity.Roles.Contracts;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Contexts;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

public class RoleService:IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantInfoContextAccessor;
    // private readonly TenantInfo
    
    private IRoleService _roleServiceImplementation;

    public RoleService(
        RoleManager<ApplicationRole> roleManager, 
        UserManager<ApplicationUser> userManager, 
        ApplicationDbContext context,
        IRoleService roleServiceImplementation,
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _roleServiceImplementation = roleServiceImplementation;
        _tenantInfoContextAccessor = tenantInfoContextAccessor;
    }

    public async Task<string> CreateAsync(CreateRoleRequest request)
    {
        var newRole = new ApplicationRole()
        {
            Name = request.Name,
            Description = request.Description
        };
        var result = await _roleManager.CreateAsync(newRole);
        
        if (!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }
        // return await _roleServiceImplementation.CreateAsync(request);
        return newRole.Name;
    }

    public async Task<string> UpdateAsync(UpdateRoleRequest request)
    {
        // return await _roleServiceImplementation.UpdateAsync(request);
        var roleInDb = await _roleManager.FindByIdAsync(request.Id)
                       ?? throw new NotFoundException(["Role does not exist"]);

        if (RoleConstants.IsDefaultRole(roleInDb.Name))
        {
            throw new ConflictException([$"Changes not allowed on system role {roleInDb.Name} role."]);
        }

        roleInDb.Name = request.Name;
        roleInDb.Description = request.Description;
        roleInDb.NormalizedName = request.Name.ToUpperInvariant();

        var result = await _roleManager.UpdateAsync(roleInDb);
        if (!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }

        return roleInDb.Name;
    }

    public async Task<string> DeleteAsync(string id)
    {
        // return await _roleServiceImplementation.DeleteAsync(id);
        var existingRole = await _roleManager.FindByIdAsync(id)
                           ?? throw new NotFoundException(["Role does not exist"]);

        if (RoleConstants.IsDefaultRole(existingRole.Name))
        {
            throw new ConflictException([$"Not allowed to delete '{existingRole.Name}' role."]);
        }

        if ((await _userManager.GetUsersInRoleAsync(existingRole.Name)).Count > 0)
        {
            throw new ConflictException([$"Not allowed to delete '{existingRole.Name}' " +
                                         $"role as is currently assigned to user."]);
        }

        var result = await _roleManager.DeleteAsync(existingRole);
        if (!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }

        return existingRole.Name;
    }

    public async Task<string> UpdatePermissionsAsync(UpdateRolePermissionRequest request)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId)
                   ?? throw new NotFoundException(["Role does not exist"]);

        if (role.Name == RoleConstants.Admin)
        {
            throw new ConflictException([$"Not allowed to change permissions for '{role.Name}' role."]);
        }

        if (_tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Id != TenancyConstants.Root.Id)
        {
            request.NewPermissions.RemoveAll(p => p.StartsWith("Permission.Tenants."));
        }

        var currentClaimns = await _roleManager.GetClaimsAsync(role);
        foreach (var claim in currentClaimns.Where(c => !request.NewPermissions.Any(p=>p==c.Value)))
        {
            var result = await _roleManager.RemoveClaimAsync(role, claim);

            if (!result.Succeeded)
            {
                throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
            }
        }
        foreach (var newPermission in request.NewPermissions.Where(p=>!currentClaimns.Any(c=>c.Value ==p)))
        {
            await _context
                .RoleClaims.AddAsync(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = newPermission,
                    Description = "",
                    Group = ""
                });
        }
        await _context.SaveChangesAsync();
        return "Permissions Updated Successfully";
    }

    public async Task<bool> DoesItExistAsync(string name)
    {
        return await _roleManager.RoleExistsAsync(name);
    }

    public async Task<List<RoleResponse>> GetAllAsync(CancellationToken ct)
    {
        var existingRoles = await _roleManager.Roles.ToListAsync(ct);
        return existingRoles.Adapt<List<RoleResponse>>();
    }

    public async Task<RoleResponse> GetByIdAsync(string id, CancellationToken ct)
    {
        var rolesInDb = await _context.Roles.FirstOrDefaultAsync(
            role => role.Id == id, ct) ?? throw new NotFoundException(["Role does not exist"]);
        return rolesInDb.Adapt<RoleResponse>();
    }

    public async Task<RoleResponse> GetRoleWthPermissionsAsync(string id, CancellationToken ct)
    {
        // return await _roleServiceImplementation.GetRoleWthPermissionsAsync(id, ct);
        var role = await GetByIdAsync(id, ct);
        role.Permissions = await _context.RoleClaims.Where(rc => rc.RoleId == id && rc.ClaimType ==
                ClaimConstants.Permission)
            .Select(rc => rc.ClaimValue)
            .ToListAsync(ct);
        return role;
    }

    
}
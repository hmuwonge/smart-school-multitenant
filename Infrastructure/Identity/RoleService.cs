using Application.Exceptions;
using Application.Features.Identity.Roles;
using Application.Features.Identity.Roles.Contracts;
using Infrastructure.Constants;
using Infrastructure.Contexts;
using Infrastructure.Identity.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

public class RoleService:IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    
    private IRoleService _roleServiceImplementation;

    public RoleService(
        RoleManager<ApplicationRole> roleManager, 
        UserManager<ApplicationUser> userManager, 
        ApplicationDbContext context,
        IRoleService roleServiceImplementation)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _roleServiceImplementation = roleServiceImplementation;
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
            throw new IdentityException(GetIdentityResultErrorDescriptions(result));
        }
        // return await _roleServiceImplementation.CreateAsync(request);
        return newRole.Name;
    }

    public async Task<string> UpdateAsync(UpdateRoleRequest request)
    {
        return await _roleServiceImplementation.UpdateAsync(request);
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
            throw new IdentityException(GetIdentityResultErrorDescriptions(result));
        }

        return existingRole.Name;
    }

    public async Task<string> UpdatePermissionsAsync(UpdateRolePermissionRequest request)
    {
        return await _roleServiceImplementation.UpdatePermissionsAsync(request);
    }

    public async Task<bool> DoesItExistAsync(string name)
    {
        return await _roleManager.RoleExistsAsync(name);
    }

    public async Task<List<RoleResponse>> GeAllAsync(CancellationToken ct)
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
        return await _roleServiceImplementation.GetRoleWthPermissionsAsync(id, ct);
    }

    private List<string> GetIdentityResultErrorDescriptions(IdentityResult identityResult)
    {
        // return identityResult.Errors.Select(errror => errror.Description).ToList();
        var errorDescriptions = new List<string>();

        foreach (var error in identityResult.Errors)
        {
            errorDescriptions.Add(error.Description);
        }
        return errorDescriptions;
    }
}
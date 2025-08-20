using Application.Exceptions;
using Application.Features.Identity.Users.Contracts;
using Application.Features.Identity.Users.Requests;
using Application.Features.Identity.Users.Response;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Contexts;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserRolesRequest = Application.Features.Identity.Users.Requests.UserRolesRequest;

namespace Infrastructure.Identity;

public class UserService: IUserService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantInfoContextAccessor;
    private IUserService _userServiceImplementation;

    public UserService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor, IUserService userServiceImplementation)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _tenantInfoContextAccessor = tenantInfoContextAccessor;
        _userServiceImplementation = userServiceImplementation;
    }

    /// <summary>
    /// Creates a new user with the provided details.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// Returns the ID of the newly created user.</returns>
    /// <exception cref="ConflictException"></exception>
    /// <exception cref="IdentityException"></exception>
    public async Task<string> CreateAsync(CreateUserRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            throw new ConflictException(["Passwords do not match"]);
        }

        if (await IsEmailTakenAsync(request.Email))
        {
            throw new ConflictException(["Email is already taken"]);
        }
        var newUser = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true,
            IsActive = request.IsActive
        };
        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }

        return newUser.Id;
    }

    /// <summary>
    /// Updates the details of an existing user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Returns the ID of the updated user.</returns>
    public async Task<string> UpdateAsync(UpdateUserRequest request)
    {
        var existingUser = await GetUserAsync(request.Id);
        existingUser.FirstName = request.FirstName;
        existingUser.LastName = request.LastName;
        existingUser.PhoneNumber = request.PhoneNumber;

        var result = await _userManager.UpdateAsync(existingUser);
        if (!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }

        return existingUser.Id;
    }

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Returns the ID of the deleted user.</returns>
    /// <exception cref="ConflictException"></exception>
    public async Task<string> DeleteAsync(string userId)
    {
        var existingUser = await GetUserAsync(userId);

        if (existingUser.Email == TenancyConstants.Root.Email)
        {
            throw new ConflictException(["Not allowed to delete Root Admin User for Tenant."]);
        }

        _context.Users.Remove(existingUser);
        await _context.SaveChangesAsync();
        return userId;
    }

    /// <summary>
    /// Activates or deactivates a user account.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="activation"></param>
    /// <returns>Returns the ID of the affected user.</returns>
    /// <exception cref="NotFoundException"></exception>
    /// <exception cref="IdentityException"></exception>
    public async Task<string> ActivateOrDeactivateAsync(string userId, bool activation)
    {
        var userInDb = await _userManager.FindByIdAsync(userId)
                       ?? throw new NotFoundException(["User does not exist"]);

        userInDb.IsActive = activation;
        var result = await _userManager.UpdateAsync(userInDb);

        if (!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }

        return userId;
    }

    /// <summary>
    /// Changes the password for a user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns> Returns the ID of the user whose password was changed.</returns>
    /// <exception cref="ConflictException"></exception>
    /// <exception cref="IdentityException"></exception>
    public async Task<string> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var existingUser = await GetUserAsync(request.UserId);

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            throw new ConflictException(["Passwords do not match"]);
        }

        var result = await _userManager.ChangePasswordAsync(existingUser, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }

        return existingUser.Id;
    }

    /// <summary>
    /// Assigns or removes roles for a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <returns>Returns the ID of the user whose roles were updated.</returns>
    /// <exception cref="ConflictException"></exception>
    public async Task<string> AssignRolesAsync(string userId, UserRolesRequest request)
    {
        var userInDb = await GetUserAsync(userId);
        var userWithRole = await _userManager.IsInRoleAsync(userInDb, RoleConstants.Admin);
        if (userWithRole && request.UserRoles.Any(ur => !ur.IsAssigned && ur.Name == RoleConstants.Admin))
        {
            var adminUsersCount = (await _userManager.GetUsersInRoleAsync(RoleConstants.Admin)).Count;
            if (userInDb.Email == TenancyConstants.Root.Email)
            {
                if (_tenantInfoContextAccessor.MultiTenantContext.TenantInfo?.Id == TenancyConstants.Root.Id)
                {
                    throw new ConflictException(["Not allowed to remove Admin role for a Root Tenant User."]);
                }
            }
            else if (adminUsersCount <= 2)
            {
                throw new ConflictException(["Not allowed. Tenant should have at least two Admin Users."]);
            }
        }

        foreach (var userRole in request.UserRoles)
        {
            if (userRole.IsAssigned)
            {
                if (!await _userManager.IsInRoleAsync(userInDb,userRole.Name))
                {
                    await _userManager.AddToRoleAsync(userInDb, userRole.Name);
                }
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(userInDb,userRole.Name);
            }
        }

        return userId;
    }

    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns>Returns a list of user responses.</returns>
    public async Task<List<UserResponse>> GetAllAsync(CancellationToken ct)
    {
        var users = await _userManager.Users.ToListAsync(ct);
        return users.Adapt<List<UserResponse>>();
    }

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns>Returns the user response.</returns>
    public async Task<UserResponse> GetByIdAsync(string userId, CancellationToken ct)
    {
        var userInDb = await GetUserAsync(userId);
        return userInDb.Adapt<UserResponse>();
    }

    /// <summary>
    /// Retrieves all roles assigned to a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns>Returns a list of user role responses.</returns>
    public async Task<List<UserRoleResponse>> GetUserRolesAsync(string userId, CancellationToken ct)
    {
        var existingUser = await GetUserAsync(userId);
        var userRoles = new List<UserRoleResponse>();
        var rolesInDb = await _roleManager.Roles.ToListAsync(ct);

        foreach (var role in rolesInDb)
        {
            userRoles.Add(
                new UserRoleResponse
                {
                    RoleId = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsAssigned = role.Name != null && await _userManager.IsInRoleAsync(existingUser, role.Name)
                });
        }

        return userRoles;
    }

    /// <summary>
    /// Checks if an email is already taken by another user.
    /// </summary>
    /// <param name="email"></param>
    /// <returns> Returns true if the email is taken, otherwise false.</returns>
    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) is not null;
    }

    /// <summary>
    /// Retrieves all permissions assigned to a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns>Returns a list of permission strings.</returns>
    public async Task<List<string>> GetUserPermissionsAsync(string userId, CancellationToken ct)
    {
        var existingUser = await GetUserAsync(userId);
        var userRoleNames = await _userManager.GetRolesAsync(existingUser);
        var permissions = new List<string>();

        foreach (var role in await _roleManager
                     .Roles.Where(r=> r.Name != null && userRoleNames.Contains(r.Name)).ToListAsync(ct))
        {
            permissions.AddRange(await _context.RoleClaims
                .Where(rc=>rc.RoleId == role.Id && rc.ClaimType == ClaimConstants.Permission)
                .Select(rc=>rc.ClaimValue).ToListAsync(ct));
        }
        return permissions.Distinct().ToList();
    }

    /// <summary>
    /// Checks if a specific permission is assigned to a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="permission"></param>
    /// <param name="ct"></param>
    /// <returns>Returns true if the permission is assigned, otherwise false.</returns>
    public async Task<bool> IsPermissionAssignedAsync(string userId, string permission, CancellationToken ct = default)
    {
        return (await GetUserPermissionsAsync(userId, ct)).Contains(permission);
    }

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Returns the ApplicationUser entity.</returns>
    /// <exception cref="NotFoundException"></exception>
    private async Task<ApplicationUser> GetUserAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId)
               ?? throw new NotFoundException(["User does not exist."]);
    }
}
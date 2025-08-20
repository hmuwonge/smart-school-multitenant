using Application.Features.Identity.Users.Commands;
using Application.Features.Identity.Users.Queries;
using Application.Features.Identity.Users.Requests;
using Infrastructure.Constants;
using Infrastructure.Identity.Auth;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class UsersController : BaseApiController
{
    // GET
    [HttpPost("register")]
    [ShouldHavePermission(SchoolAction.Create, SchoolFeature.Users)]
    public async  Task<IActionResult> RegisterUserAsync([FromBody] CreateUserRequest createUser)
    {
        var response = await Sender.Send(new CreateUserCommand
        {
            CreateUser = createUser
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPut("update")]
    [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Users)]
    public async Task<IActionResult> UpdateUserDetailsAsync([FromBody] UpdateUserRequest update)
    {
        var response = await Sender.Send(new UpdateUserCommand
        {
            UpdateUser = update
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
    
    [HttpPut("update-status")]
    [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Users)]
    public async Task<IActionResult> UpdateUserStatusAsync([FromBody] ChangeUserStatusRequest update)
    {
        var response = await Sender.Send(new UpdateUserStatusCommand
        {
            ChangeUserStatus = update
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
    
    [HttpPut("update-roles/{roleId}")]
    [ShouldHavePermission(SchoolAction.Update, SchoolFeature.UserRoles)]
    public async Task<IActionResult> UpdateUserRolesAsync([FromBody] UserRolesRequest update, string roleId )
    {
        var response = await Sender.Send(new UpdateUserRolesCommand
        {
            UserRoleRequest = update,
            RoleId = roleId
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
    
    [HttpDelete("delete/{userId}")]
    [ShouldHavePermission(SchoolAction.Delete, SchoolFeature.Users)]
    public async Task<IActionResult> DeleteUserAsync(string userId)
    {
        var response = await Sender.Send(new DeleteUserCommand
        {
            UserId = userId
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
    
    
    [HttpGet("all")]
    [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Users)]
    public async Task<IActionResult> GetUsersAsync()
    {
        var response = await Sender.Send(new GetAllUsersQuery());

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
    
    [HttpGet("{userId}")]
    [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Users)]
    public async Task<IActionResult> GetUserByIdAsync(string userId)
    {
        var response = await Sender.Send(new GetUserByIdQuery
        {
            UserId = userId
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
    
    [HttpGet("permissions/{userId}")]
    [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Users)]
    public async Task<IActionResult> GetUserPermissionsAsync(string userId)
    {
        var response = await Sender.Send(new GetUserPermissionsQuery
        {
            UserId = userId
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
    
    [HttpGet("user-roles/{userId}")]
    [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Users)]
    public async Task<IActionResult> GetUserRolesAsync(string userId)
    {
        var response = await Sender.Send(new GetUserRolesQuery
        {
            UserId = userId
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
    
}
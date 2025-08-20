using Application.Features.Identity.Roles;
using Application.Features.Identity.Roles.Commands;
using Application.Features.Identity.Users.Commands;
using Application.Features.Identity.Users.Requests;
using Infrastructure.Constants;
using Infrastructure.Identity.Auth;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class RolesController : BaseApiController
{
    // GET
    [HttpPost("create")]
    [ShouldHavePermission(SchoolAction.Create, SchoolFeature.Roles)]
    public async  Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleRequest createRole)
    {
        var response = await Sender.Send(new CreateRoleCommand
        {
            CreateRole = createRole
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    
    [HttpPut("update")]
    [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Roles)]
    public async Task<IActionResult> UpdateRoleAsync([FromBody] UpdateRoleRequest update)
    {
        var response = await Sender.Send(new UpdateRoleCommand
        {
            UpdateRole = update
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
    
    [HttpDelete("delete/{roleId}")]
    [ShouldHavePermission(SchoolAction.Delete, SchoolFeature.Roles)]
    public async Task<IActionResult> DeleteUserAsync(string roleId)
    {
        var response = await Sender.Send(new DeleteRoleCommand
        {
            RoleId = roleId
        });
      if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
    
    [HttpPut("update-permissions/{roleId}")]
    [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Roles)]
    public async Task<IActionResult> UpdateUserRolesAsync([FromBody] UpdateRolePermissionRequest update, string roleId )
    {
        var response = await Sender.Send(new UpdateRolePermissionsCommand
        {
            UpdateRolePermission = update,
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
}
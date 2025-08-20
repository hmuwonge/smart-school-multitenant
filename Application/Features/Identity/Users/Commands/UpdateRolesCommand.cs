using Application.Features.Identity.Users.Contracts;
using Application.Features.Identity.Users.Requests;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;

public class UpdateUserRolesCommand: IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
    public UserRolesRequest UserRoleRequest { get; set; }
}


public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand,IResponseWrapper>
{
    private readonly IUserService _userService;

    public UpdateUserRolesCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        // Logic to create a user would go here
        var userId = await _userService.AssignRolesAsync(request.RoleId, request.UserRoleRequest);
        // For now, we will return a success response

        return await ResponseWrapper<string>.SuccessAsync(
            data: userId,
            message: "User role updated successfully"
        );
    }
}
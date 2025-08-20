using Application.Features.Identity.Users.Contracts;
using Application.Features.Identity.Users.Requests;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;

public class UpdateUserCommand: IRequest<IResponseWrapper>
{
    public UpdateUserRequest UpdateUser { get; set; }
}


public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand,IResponseWrapper>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        // Logic to create a user would go here
        var userId = await _userService.UpdateAsync(request.UpdateUser);
        // For now, we will return a success response

        return await ResponseWrapper<string>.SuccessAsync(
            data: userId,
            message: "User updated successfully"
        );
    }
}
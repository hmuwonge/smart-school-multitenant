using Application.Features.Identity.Users.Contracts;
using Application.Features.Identity.Users.Requests;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;

public class DeleteUserCommand: IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class DeleteUserCommandHandler(IUserService userService) : IRequestHandler<DeleteUserCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public async Task<IResponseWrapper> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userService.DeleteAsync(request.UserId);
        return await ResponseWrapper<string>.SuccessAsync(
            data: userId,
            message: "User deleted successfully"
        );
    }
}
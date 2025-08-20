using Application.Features.Identity.Users.Contracts;
using Application.Features.Identity.Users.Requests;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;

public class CreateUserCommand:IRequest<IResponseWrapper>
{
    public CreateUserRequest CreateUser { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Logic to create a user would go here
        var userId = await _userService.CreateAsync(request.CreateUser);
        // For now, we will return a success response

        return await ResponseWrapper<string>.SuccessAsync(
            data: userId,
            message: "User created successfully"
        );
    }
}
using Application.Features.Identity.Users.Contracts;
using Application.Features.Identity.Users.Response;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Queries;

public class GetUserByIdQuery: IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, IResponseWrapper>
{
    private readonly IUserService _userService;

    public GetUserByIdQueryHandler(IUserService userService)
    {
        _userService = userService;
    }


    public async Task<IResponseWrapper> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(request.UserId, cancellationToken);
        return await ResponseWrapper<UserResponse>.SuccessAsync(data: user
        );
    }
}
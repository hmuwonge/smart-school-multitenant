using Application.Features.Identity.Users.Contracts;
using Application.Features.Identity.Users.Response;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Queries;

public class GetUserRolesQuery:IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class GetUserRolesQueryHandler(IUserService userService) : IRequestHandler<GetUserRolesQuery, IResponseWrapper>
{
    private readonly IUserService _userService = userService;

    // public GetUserRolesQueryHandler(IUserService userService)
    // {
    //     _userService = userService;
    // }

    public async Task<IResponseWrapper> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _userService.GetUserRolesAsync(request.UserId, cancellationToken);
        return await ResponseWrapper<List<UserRoleResponse>>.SuccessAsync(data: roles);
    }
}
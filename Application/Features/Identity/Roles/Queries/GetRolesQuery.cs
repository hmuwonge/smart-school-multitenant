using Application.Features.Identity.Roles.Contracts;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Queries;

public class GetRolesQuery:IRequest<IResponseWrapper>
{
}

public class GetRolesQueryHandler(IRoleService roleService):IRequestHandler<GetRolesQuery, IResponseWrapper>
{
    private readonly IRoleService _roleService = roleService;
    public async Task<IResponseWrapper> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllAsync(cancellationToken);
        return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(data: roles);
    }
}
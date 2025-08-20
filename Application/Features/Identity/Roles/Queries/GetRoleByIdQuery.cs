using Application.Features.Identity.Roles.Contracts;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Queries;

public class GetRoleByIdQuery:IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
}

public class GetRoleByIdCommand(IRoleService roleService):IRequestHandler<GetRoleByIdQuery, IResponseWrapper>
{
    private readonly IRoleService _roleService = roleService;
    public async Task<IResponseWrapper> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetByIdAsync(request.RoleId, cancellationToken);
        return await ResponseWrapper<RoleResponse>.SuccessAsync(data: role);
    }
}
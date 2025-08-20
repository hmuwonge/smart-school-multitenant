using Application.Features.Identity.Roles.Contracts;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Commands;

public class DeleteRoleCommand :IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
    
    public class DeleteRoleCommandHandler: IRequestHandler<DeleteRoleCommand, IResponseWrapper>
    {
        private readonly IRoleService _roleService;

        public DeleteRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IResponseWrapper> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var roleName = await _roleService.DeleteAsync(request.RoleId);
            return await ResponseWrapper<string>.SuccessAsync(message: $"Role '{roleName}' deleted successfully.");
        }
    }
}
using Application.Features.Identity.Roles.Contracts;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Commands;

public class UpdateRolePermissionsCommand :IRequest<IResponseWrapper>
{
    public UpdateRolePermissionRequest UpdateRolePermission { get; set; }
    
    public class UpdateRolePermissionCommandHandler: IRequestHandler<UpdateRolePermissionsCommand, IResponseWrapper>
    {
        private readonly IRoleService _roleService;

        public UpdateRolePermissionCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IResponseWrapper> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
        {
            var message = await _roleService.UpdatePermissionsAsync(request.UpdateRolePermission);
            return await ResponseWrapper.SuccessAsync(message: message);
        }
    }
}
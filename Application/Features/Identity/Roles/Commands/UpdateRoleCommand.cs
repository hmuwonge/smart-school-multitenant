using Application.Features.Identity.Roles.Contracts;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Commands;

public class UpdateRoleCommand :IRequest<IResponseWrapper>
{
    public UpdateRoleRequest UpdateRole { get; set; }
    
    public class UpdateRoleCommandHandler: IRequestHandler<UpdateRoleCommand, IResponseWrapper>
    {
        private readonly IRoleService _roleService;

        public UpdateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IResponseWrapper> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var roleName = await _roleService.UpdateAsync(request.UpdateRole);
            return await ResponseWrapper<string>.SuccessAsync(message: $"Role '{roleName}' updated successfully.");
        }
    }
}
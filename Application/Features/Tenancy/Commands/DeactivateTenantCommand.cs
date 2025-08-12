using Application.Features.Tenancy;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Tenancy.Commands
{
    public class DeactivateTenantCommand:IRequest<IResponseWrapper>
    {
        public string TenantId { get; set; }
    }

    public class DeactivateTenantCommandHandler : IRequestHandler<DeactivateTenantCommand, IResponseWrapper>
    {
        private readonly ITenantService _tenantService;
        
        public DeactivateTenantCommandHandler(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }
        
        public async Task<IResponseWrapper> Handle(DeactivateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenantId = await _tenantService.DeactivateAsync(request.TenantId);
            return await ResponseWrapper<string>.SuccessAsync(data: tenantId, "Tenant de-acivation successfully");
        }
    }
}

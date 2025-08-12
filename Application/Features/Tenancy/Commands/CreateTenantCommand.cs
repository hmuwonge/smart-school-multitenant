using Application.Features.Tenancy;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Tenancy.Commands
{
    public class CreateTenantCommand:IRequest<IResponseWrapper>
    {
        public CreateTenantRequest CreateTenant { get; set; }
    }

    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, IResponseWrapper>
    {
        private readonly ITenantService _tenantService;
        
        public CreateTenantCommandHandler(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }
        
        public async Task<IResponseWrapper> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenantId = await _tenantService.CreateTenantAsync(request.CreateTenant, cancellationToken);
            return await ResponseWrapper<string>.SuccessAsync(data: tenantId,"Tenant created successfully");
        }
    }
}

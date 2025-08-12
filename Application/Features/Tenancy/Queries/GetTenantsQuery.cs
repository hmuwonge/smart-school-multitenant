using Application.Features.Tenancy;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Tenancy.Queries
{
    public class GetTenantsQuery:IRequest<IResponseWrapper>
    {
    }

    public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, IResponseWrapper>
    {
      private readonly ITenantService _tenantService;

        public GetTenantsQueryHandler(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public async Task<IResponseWrapper> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
        {
            var tenants = await _tenantService.GetTenantsAsync();
            return await ResponseWrapper<List<TenantResponse>>.SuccessAsync(data: tenants);
        }
    }
}

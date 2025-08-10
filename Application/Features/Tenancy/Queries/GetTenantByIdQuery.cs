using Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tenancy.Queries
{
    public class GetTenantByIdQuery:IRequest<IResponseWrapper>
    {
        public string TenantId { get; set; }
    }

    public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, IResponseWrapper>
    {
        private readonly ITenantService _tenantService;
        public async Task<IResponseWrapper> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
        {
           var tenant = await _tenantService.GetTenantByIdAsync(request.TenantId);

            if (tenant != null)
            {
                return await ResponseWrapper<TenantResponse>.SuccessAsync(data: tenant);
            }
            return await ResponseWrapper<TenantResponse>.FailAsync(message: "Tenant doesnot exist.");
        }
    }
}
 
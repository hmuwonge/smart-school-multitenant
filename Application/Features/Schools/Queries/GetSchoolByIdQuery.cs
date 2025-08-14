using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Schools.Queries;

public class GetSchoolByIdQuery:IRequest<IResponseWrapper>
{
    public int SchoolId { get; set; }
}

public class GetSchoolIdQueryHandler : IRequestHandler<GetSchoolByIdQuery, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;

    public GetSchoolIdQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IResponseWrapper> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        var schoolInDb = await _schoolService.GetByIdAsync(request.SchoolId);
        if (schoolInDb is not null)
        {
            return await ResponseWrapper<SchoolResponse>.SuccessAsync(data: schoolInDb.Adapt<SchoolResponse>());
        }
        return await ResponseWrapper<int>.FailAsync("School does not exist.");
    }
}

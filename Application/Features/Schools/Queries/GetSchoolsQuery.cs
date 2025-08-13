using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Schools.Queries;

public class GetSchoolsQuery: IRequest<IResponseWrapper>
{
}

public class GetSchoolsQueryHandler : IRequestHandler<GetSchoolsQuery, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;

    public GetSchoolsQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IResponseWrapper> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        var schoolInDb = await _schoolService.GetAllAsync();
        if (schoolInDb?.Count > 0)
        {
            return await ResponseWrapper<List<SchoolResponse>>.SuccessAsync(
                data: schoolInDb.Adapt<List<SchoolResponse>>());
        }

        return await ResponseWrapper<int>.FailAsync("No Schools were found.");
    }
}
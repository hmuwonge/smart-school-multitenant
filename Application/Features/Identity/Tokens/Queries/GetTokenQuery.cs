using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Tokens.Queries;

public class GetTokenQuery: IRequest<IResponseWrapper>
{
    public TokenRequest TokenRequest { get; set; }
}

public class GetTokenQueryHandler : IRequestHandler<GetTokenQuery, IResponseWrapper>
{
    private readonly ITokenService _tokenService;

    public GetTokenQueryHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<IResponseWrapper> Handle(GetTokenQuery request, CancellationToken cancellationToken)
    {
        var token = await _tokenService.LoginAsync(request.TokenRequest);
        return await ResponseWrapper<TokenResponse>.SuccessAsync(data: token);
    }
}
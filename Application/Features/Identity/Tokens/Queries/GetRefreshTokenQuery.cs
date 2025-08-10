using Application.Wrappers;
using MediatR;

namespace Application.Features.Identity.Tokens.Queries;

public class GetRefreshTokenQuery:IRequest<IResponseWrapper>
{
    public RefreshTokenRequest RefreshToken { get; set; }
}

public class GetRefreshTokenQueryHandler : IRequestHandler<GetRefreshTokenQuery, IResponseWrapper>
{
    private readonly ITokenService _tokenService;

    public GetRefreshTokenQueryHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<IResponseWrapper> Handle(GetRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var refreshToken = await _tokenService.RefreshTokenAsync(request.RefreshToken);
        return await ResponseWrapper<TokenResponse>.SuccessAsync(data: refreshToken);
    }
}
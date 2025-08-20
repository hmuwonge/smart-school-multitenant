using Application.Features.Identity.Users.Contracts;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Identity.Middleware;

public class CurrentUserMiddleware: IMiddleware
{
    private readonly ICurrentUserService _currentUser;

    public CurrentUserMiddleware(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _currentUser.SetCurrentUser(context.User);
        await next(context);
    }
}
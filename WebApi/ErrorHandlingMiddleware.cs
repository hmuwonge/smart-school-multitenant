using Application.Exceptions;
using Application.Wrappers;
using System.Net;
using System.Text.Json;

namespace WebApi
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                var responseWrapper  = ResponseWrapper.Fail(message: ex.Message);

                switch (ex)
                {
                    case ConflictException ce:
                        response.StatusCode = (int)ce.StatusCode;
                        responseWrapper.Messages = ce.ErrorMessages;
                        break;
                    case NotFoundException nfe:
                        response.StatusCode = (int)nfe.StatusCode;
                        responseWrapper.Messages = nfe.ErrorMessages;
                        break;
                    case ForbiddenException fe:
                        response.StatusCode = (int)fe.StatusCode;
                        responseWrapper.Messages = fe.ErrorMessages;
                        break;
                    case IdentityException ie:
                        response.StatusCode = (int)ie.StatusCode;
                        responseWrapper.Messages = ie.ErrorMessages;
                        break;
                    case UnauthorizedException ue:
                        response.StatusCode = (int)ue.StatusCode;
                        responseWrapper.Messages = ue.ErrorMessages;
                        break;
                   default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseWrapper.Messages = ["Something went wrong. Contact Administration."];
                        break;
                }

                response.StatusCode = ex switch
                {
                    ConflictException ce => (int)ce.StatusCode,
                    NotFoundException nfe => (int)nfe.StatusCode,
                    ForbiddenException fe => (int)fe.StatusCode,
                    IdentityException ie => (int)ie.StatusCode,
                    UnauthorizedException ue => (int)ue.StatusCode,
                    _ => (int)HttpStatusCode.InternalServerError,
                };

                var result = JsonSerializer.Serialize(responseWrapper);
                await response.WriteAsync(result);
            }
        }
    }
}

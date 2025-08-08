using System.Net;

namespace Application.Exceptions;

public class UnauthorizedException : Exception
{
    public List<string> ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public UnauthorizedException(List<string> errorMessage = default,HttpStatusCode statusCode = HttpStatusCode.Unauthorized
        )
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
}
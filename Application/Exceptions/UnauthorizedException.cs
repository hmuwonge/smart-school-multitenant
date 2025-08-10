using System.Net;

namespace Application.Exceptions;

public class UnauthorizedException : Exception
{
    public List<string> ErrorMessages { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public UnauthorizedException(HttpStatusCode statusCode = HttpStatusCode.Unauthorized, List<string> errorMessage = default
        )
    {
        StatusCode = statusCode;
        ErrorMessages = errorMessage;
    }
}
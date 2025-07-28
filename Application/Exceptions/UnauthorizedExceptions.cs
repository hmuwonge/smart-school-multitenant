using System.Net;

namespace Application.Exceptions;

public class UnauthorizedExceptions : Exception
{
    public List<string> ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public UnauthorizedExceptions(HttpStatusCode statusCode = HttpStatusCode.Unauthorized,
        List<string> errorMessage = default)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
}
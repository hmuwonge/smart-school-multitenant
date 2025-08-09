using System.Net;

namespace Application.Exceptions;

public class IdentityException
{
    public List<string> ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public IdentityException(HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
        List<string> errorMessage = default)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
}
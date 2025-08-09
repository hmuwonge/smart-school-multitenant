using System.Net;

namespace Application.Exceptions;

public class ConflictException
{
    public List<string> ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public ConflictException(HttpStatusCode statusCode = HttpStatusCode.Conflict,
        List<string> errorMessage = default)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
}
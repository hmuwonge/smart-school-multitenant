using System.Net;

namespace Application.Exceptions;

public class ForbiddenException:Exception
{
    public List<string> ErrorMessages { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public ForbiddenException(HttpStatusCode statusCode = HttpStatusCode.Forbidden,
        List<string> errorMessage = default)
    {
        StatusCode = statusCode;
        ErrorMessages = errorMessage;
    }
}
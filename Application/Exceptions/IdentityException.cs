using System.Net;

namespace Application.Exceptions;

public class IdentityException:Exception
{
    public List<string> ErrorMessages { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public IdentityException(HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
        List<string> errorMessage = default)
    {
        StatusCode = statusCode;
        ErrorMessages = errorMessage;
    }
}
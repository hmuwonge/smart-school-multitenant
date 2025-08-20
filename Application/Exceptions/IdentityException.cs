using System.Net;

namespace Application.Exceptions;

public class IdentityException:Exception
{
    public List<string> ErrorMessages { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public IdentityException( List<string> errorMessage = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError
       )
    {
        StatusCode = statusCode;
        ErrorMessages = errorMessage;
    }
}
using System.Net;

namespace Application.Exceptions;

public class NotFoundException:Exception
{
    public List<string> ErrorMessages { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public NotFoundException(HttpStatusCode statusCode = HttpStatusCode.NotFound,
        List<string> errorMessages = default)
    {
        StatusCode = statusCode;
        ErrorMessages = errorMessages;
    }
    
}
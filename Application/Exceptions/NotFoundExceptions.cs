using System.Net;

namespace Application.Exceptions;

public class NotFoundException:Exception
{
    public List<string> ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public NotFoundException(HttpStatusCode statusCode = HttpStatusCode.NotFound,
        List<string> errorMessage = default)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
    
}
using System.Net;

namespace Application.Exceptions;

public class NotFoundException:Exception
{
    public List<string> ErrorMessages { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public NotFoundException(List<string> errorMessages = default,HttpStatusCode statusCode = HttpStatusCode.NotFound
        )
    {
        ErrorMessages = errorMessages;
        StatusCode = statusCode;
        
    }
    
}
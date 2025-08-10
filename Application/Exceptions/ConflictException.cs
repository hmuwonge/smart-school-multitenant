using System.Net;

namespace Application.Exceptions;

public class ConflictException:Exception
{
    public List<string> ErrorMessages { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public ConflictException(List<string> errorMessages = default,HttpStatusCode statusCode = HttpStatusCode.Conflict)
    {
        ErrorMessages = errorMessages;
        StatusCode = statusCode;        
    }
}
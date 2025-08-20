namespace Application.Features.Identity.Users.Requests;

public class ChangeUserStatusRequest
{
    public string UserId { get; set; }
    public string Activation { get; set; }  
}
namespace Application.Features.Identity.Users.Requests;

public class ChangePasswordRequest
{
    public string UserId { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}
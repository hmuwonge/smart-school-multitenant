namespace Application.Features.Identity.Users.Requests;

public class UpdateUserRequest
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
}
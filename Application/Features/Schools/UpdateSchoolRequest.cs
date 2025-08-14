namespace Application.Features.Schools;

public class UpdateSchoolRequest
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime EstablishedDate { get; set; }
    public DateTime CreatedOn { get; set; }
}
namespace Domain.Entities
{
    public class School
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime EstablishedDate { get; set; }
        public DateTime CreatedOn { get; set; }
        //public string PhoneNumber { get; set; }
        //public string Email { get; set; }
        //public string Website { get; set; }
        //// Navigation properties
        //public ICollection<Student> Students { get; set; } = new List<Student>();
        //public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}

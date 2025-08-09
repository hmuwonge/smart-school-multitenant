using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Models
{
    public class ApplicationRole: IdentityRole
    {
        public string? Description { get; set; } // Optional description for the role
        public string? Group { get; set; } // Optional group for categorizing roles
        //public bool IsActive { get; set; } = true; // Default to active
        // You can add more properties as needed, such as permissions, etc.
        // public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}

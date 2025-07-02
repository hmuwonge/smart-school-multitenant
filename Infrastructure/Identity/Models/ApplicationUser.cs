using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string FullName => $"{FirstName} {LastName}";
        //public string? TenantId { get; set; } // Nullable to allow for non-tenant users
        public string RefreshToken { get; set; } // Default to empty string
        public DateTime RefreshTokenExpiryTime { get; set; } // Default to 30 days from now
        public bool IsActive { get; set; } // Default to active
    }
}

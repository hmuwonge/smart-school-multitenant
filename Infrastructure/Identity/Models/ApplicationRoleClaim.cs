using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Models
{
    public class ApplicationRoleClaim: IdentityRoleClaim<string>
    {
        public string Description { get; set; } // Optional description for the role
        public string Group { get; set; } // Optional group for categorizing roles
    }
}

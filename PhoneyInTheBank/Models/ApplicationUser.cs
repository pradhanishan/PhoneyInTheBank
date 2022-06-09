

using Microsoft.AspNetCore.Identity;

namespace PhoneyInTheBank.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? City { get; set; }
    }
}

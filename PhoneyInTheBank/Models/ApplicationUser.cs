

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PhoneyInTheBank.Models
{
    public class ApplicationUser : IdentityUser
    {


        [Required, Range(16, 99)]
        public int Age { get; set; }

        [Required, MinLength(4), MaxLength(256)]
        public string? Country { get; set; }

        [Required, MinLength(4), MaxLength(256)]
        public string? City { get; set; }
    }
}

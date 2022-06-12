

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PhoneyInTheBank.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required, Display(Name = "first name"), MinLength(2), MaxLength(26)]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "last name"), MinLength(2), MaxLength(26)]
        public string LastName { get; set; } = string.Empty;

        [Required, Range(16, 99)]
        public int Age { get; set; }

        [Required, MinLength(4), MaxLength(256)]
        public string Country { get; set; } = string.Empty;

        [Required, MinLength(4), MaxLength(256)]
        public string City { get; set; } = string.Empty;

        [Required]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        [Required]
        public DateTimeOffset LastUpdatedDate { get; set; } = DateTimeOffset.Now;

        [Required]

        public bool ActiveFlag { get; set; } = true;


    }
}

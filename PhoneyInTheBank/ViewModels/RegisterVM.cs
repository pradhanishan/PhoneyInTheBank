using Microsoft.AspNetCore.Mvc.Rendering;
using PhoneyInTheBank.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace ViewModels
{
    public class RegisterVM
    {
        [Required, Display(Name = "first name"), MinLength(2), MaxLength(26)]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "last name"), MinLength(2), MaxLength(26)]
        public string? LastName { get; set; }

        [Required, MaxLength(256), DataType(DataType.EmailAddress), Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required, Range(16, 99)]
        public int Age { get; set; }

        [Required, MaxLength(20), DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, MaxLength(20), DataType(DataType.Password), Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required, MinLength(10), MaxLength(10), Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required, MinLength(4), MaxLength(256)]
        public string City { get; set; } = string.Empty;

    }
}

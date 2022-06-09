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
        [Required, MinLength(3), MaxLength(256)]
        public string? UserName { get; set; }

        [Required, MaxLength(256), DataType(DataType.EmailAddress), Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Required, Range(16, 99)]
        public int Age { get; set; }

        [Required, MaxLength(20), DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required, MaxLength(20), DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        [Required, MinLength(10), MaxLength(10)]
        public string? PhoneNumber { get; set; }

        [Required]
        public string Country { get; set; }

        [Required, MinLength(4), MaxLength(256)]
        public string? City { get; set; }

    }
}

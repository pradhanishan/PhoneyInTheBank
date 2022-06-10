using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class LoginVM
    {
        [Required, Display(Name = "Email Address"), MinLength(3), MaxLength(256)]
        public string? Email { get; set; }

        [Required, DataType(DataType.Password), MinLength(6), MaxLength(20)]
        public string? Password { get; set; }

        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class UserBankAccountVM
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
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, MinLength(10), MaxLength(15)]
        public int AccountNumber { get; set; }

        [Required, Range(0.0, float.MaxValue)]
        public float OperativeAmount { get; set; }

        [Required, Range(0.0, float.MaxValue)]
        public float LoanAmount { get; set; }

        [Required, Range(0.0, float.MaxValue)]
        public float InvestmentAmount { get; set; }

        [Required]
        public bool BankruptFlag { get; set; } = false;

        [Required]

        public bool ActiveAccountFlag { get; set; } = true;

        [Required]

        public bool ActiveUserFlag { get; set; } = true;

        [Required]
        public DateTimeOffset UserCreatedDate { get; set; } = DateTimeOffset.Now;

        [Required]

        public DateTimeOffset UserUpdatedDate { get; set; } = DateTimeOffset.Now;

        [Required]
        public DateTimeOffset AccountCreatedDate { get; set; } = DateTimeOffset.Now;

        [Required]

        public DateTimeOffset AccountUpdatedDate { get; set; } = DateTimeOffset.Now;

    }
}

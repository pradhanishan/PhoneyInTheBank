using PhoneyInTheBank.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Loan
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public BankAccount? BankAccount { get; set; }

        [Required, Display(Name = "Loan Amount")]
        public float LoanAmount { get; set; }

        [Required, Display(Name = "Amount Left To Pay")]
        public float LeftToPay { get; set; }

        [Required, Display(Name = "Interest Rate"), Range(0, 100)]
        public float InterestRate { get; set; }

        [Required, Display(Name = "Loan Amount with Interest")]
        public float LoanAmountWithInterest { get; set; }

        [Required, Display(Name = "Amount left to pay with Interest included")]
        public float LeftToPayWithInterest { get; set; }

        [Required]
        public bool ActiveFlag { get; set; } = true;

        [Required]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        [Required]
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

    }
}

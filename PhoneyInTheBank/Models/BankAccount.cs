using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneyInTheBank.Models
{
    public class BankAccount
    {
        public int Id { get; set; }

        [Required]
        public virtual ApplicationUser? ApplicationUser { get; set; }

        [Required, MinLength(10), MaxLength(15)]
        public int AccountNumber { get; set; }

        [Required, Range(0.0, float.MaxValue)]
        public float OperativeAmount { get; set; } = 1000;

        [Required, Range(0.0, float.MaxValue)]
        public float LoanAmount { get; set; }

        [Required, Range(0.0, float.MaxValue)]
        public float InvestmentAmount { get; set; }

        [Required]
        public bool BankruptFlag { get; set; } = false;

        [Required]

        public bool ActiveFlag { get; set; } = true;

        [Required]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        [Required]

        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

    }
}

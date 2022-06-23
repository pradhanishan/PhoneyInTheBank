using PhoneyInTheBank.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Investment
    {
        [Required]
        public int Id { get; set; }

        [Required]

        public ApplicationUser? ApplicationUser { get; set; }

        [Required]
        public Organization? Organization { get; set; }

        [Required]
        public float InvestmentAmount { get; set; }

        [Required]
        public float Profit { get; set; }

        [Required]

        public float Loss { get; set; }

        [Required]
        public DateTimeOffset LastCollectedDate { get; set; } = DateTimeOffset.UtcNow;

        [Required]

        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

        [Required]

        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;



    }
}

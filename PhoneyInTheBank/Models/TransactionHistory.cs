using Microsoft.AspNetCore.Identity;
using PhoneyInTheBank.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TransactionHistory
    {

        [Required]
        public int Id { get; set; }

        [Required, MinLength(4), MaxLength(256)]
        public string User { get; set; } = string.Empty;

        [Required]
        public TransactionType? TransactionType { get; set; }

        [Required]
        public float SentAmount { get; set; }

        [Required]
        public float ReceivedAmount { get; set; }

        [Required]
        public DateTimeOffset TransactionDate { get; set; } = DateTimeOffset.Now;

    }
}

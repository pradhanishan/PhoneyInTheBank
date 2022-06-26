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

        [Required, MinLength(1), MaxLength(4)]
        public string TransactionType { get; set; } = string.Empty;

        [Required]
        public float SentAmount { get; set; }

        [Required]
        public float ReceivedAmount { get; set; }

        [Required, MaxLength(256)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public DateTimeOffset TransactionDate { get; set; } = DateTimeOffset.Now;

    }
}

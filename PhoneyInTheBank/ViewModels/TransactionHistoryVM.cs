using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class TransactionHistoryVM
    {
        [Required]
        public string User { get; set; } = string.Empty;

        public float SentAmount { get; set; }
        public float ReceivedAmount { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        public DateTimeOffset TransactionDate { get; set; }

        public string Message { get; set; } = string.Empty;


    }
}

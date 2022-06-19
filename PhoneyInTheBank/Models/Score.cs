using PhoneyInTheBank.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Score
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public virtual ApplicationUser? ApplicationUser { get; set; }


        [Required, Range(0, 100)]
        public int LuckScore { get; set; }

        [Required, Range(0, 100)]
        public int GoodWillScore { get; set; }

        [Required, Range(0, 100)]
        public int TrustScore { get; set; }

        [Required, Range(0, 100)]
        public int FinancialScore { get; set; }

        [Required]
        public bool Bankrupt { get; set; } = false;

        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;




    }
}

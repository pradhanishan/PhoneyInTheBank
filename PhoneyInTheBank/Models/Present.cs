using PhoneyInTheBank.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Present
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public virtual ApplicationUser? ApplicationUser { get; set; }

        [Required]
        [Display(Name = "Last Collected On")]
        public DateTimeOffset LastCollectedDate { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        [Display(Name = "Next Present Available On")]
        public DateTimeOffset NextPresentAvailableDate { get; set; }

        [Required]
        [Display(Name = "Is gift ready to collecct?")]
        public bool GiftAvailable { get; set; } = false;

    }
}

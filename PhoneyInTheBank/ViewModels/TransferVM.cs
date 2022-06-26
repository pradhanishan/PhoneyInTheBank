using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class TransferVM
    {
        [Required]
        public float Amount { get; set; }

        [Required, Display(Name = "Donate to AI ?")]
        public bool DonateToAI { get; set; } = false;

        [MaxLength(50)]

        public string TransferType { get; set; } = "oti";
    }
}

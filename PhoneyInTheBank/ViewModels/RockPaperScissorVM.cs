using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class RockPaperScissorVM
    {
		[Display(Name ="Wager amount")]
        public float WagerValue { get; set; }

        [Display(Name ="Select your choice")]
        public string UserChoice { get; set; } = String.Empty;

        public string AIChoice { get; set; } = string.Empty;

        public bool VictoryFlag { get; set; } = false;

        public bool TieFlag { get; set; } = false;
    }
}

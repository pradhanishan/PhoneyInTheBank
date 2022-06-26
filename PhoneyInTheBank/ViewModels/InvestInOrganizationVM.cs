using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class InvestInOrganizationVM
    {
        [Required]
        public string Organization { get; set; } = string.Empty;

        [Required, Display(Name ="Your current investment balance")]
        public float InvestmentBalance { get; set; }

        [Required, Display(Name ="Enter amout you would like to invest")]
        public float InvestmentAmount { get; set; }

    }
}

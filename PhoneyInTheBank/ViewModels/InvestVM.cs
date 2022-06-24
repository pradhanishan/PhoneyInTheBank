using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class InvestVM
    {
        [Required, Display(Name = "Organization")]
        public string OrganizationName { get; set; } = string.Empty;

        public float InvestmentAmount { get; set; }

        public bool InvestedFlag { get; set; }

        public float Profit { get; set; }

        public float Loss { get; set; }

        public DateTimeOffset LastCollectedDate { get; set; }

    }
}

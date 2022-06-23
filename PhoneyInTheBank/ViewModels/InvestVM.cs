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

    }
}

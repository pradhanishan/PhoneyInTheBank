using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TransactionType
    {
        [Required]
        public int Id { get; set; }

        [Required, MinLength(3), MaxLength(10)]
        public string Type { get; set; } = string.Empty;

        [Required, MinLength(3), MaxLength(50)]
        public string TransactionDescription { get; set; } = string.Empty;

    }
}

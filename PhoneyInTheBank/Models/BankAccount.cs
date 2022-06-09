using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneyInTheBank.Models
{
    public class BankAccount
    {
        public int Id { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}

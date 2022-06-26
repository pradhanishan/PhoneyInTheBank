using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class LoanPaymentVM
    {
        public float AmountTaken { get; set; }

        public float InterestRate { get; set; }

        public float AmountWithInterest { get; set; }

        public float AmountLeftToPay { get; set; }

        public DateTimeOffset TakenOn { get; set; } = DateTimeOffset.Now;

        public float ClearanceAmount { get; set; }

        public float OperativeAmount { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class LoanVM
    {
        public int Id { get; set; }
        public string LoanType { get; set; } = string.Empty;
        public float OperativeAmount { get; set; }
        public float LoanAmount { get; set; }
        public float InterestRate { get; set; }
        public float TotalAmount { get; set; }

    }
}

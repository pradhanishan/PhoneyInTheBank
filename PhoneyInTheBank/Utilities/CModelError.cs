using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class CModelError
    {
        public static string InsufficientFundError = "You do not have enough funds to process this requrest.";

        public static string InvalidTransferTypeError = "Invalid transfer type.";

        public static string MissingActiveBankAccountError = "This user does not have an active bank account";

        public static string InvalidRockPaperScisorChoiceError = "Invalid Choice.";

        public static string PayingMoreThanOweError = "You are paying more than you owe!";

        public static string NinetyPercentLoanPaymentThresholdError = "You can't pay more than 90 percent of your balance.";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class CTransaction
    {
        public static string TRXTypeDonate = "D";

        public static string GetDonatedLog(float donatedAmount)
        {
            return "Donated " + donatedAmount.ToString() + " phonies to AI";
        }

        public static string TRXTypeOperativeToInvestmentTransfer = "OI";

        public static string GetOperativeToInvestmentTransferLog(float transferAmount)
        {
            return "Transferred " + transferAmount.ToString() + " phonies from operative to investment account.";
        }

        public static string TRXTypeInvestmentToOperativeTransfer = "IO";

        public static string GetInvestmentToOperativeTransferLog(float transferAmount)
        {
            return "Transferred " + transferAmount.ToString() + " phonies from investment to operative account.";
        }

        public static string TransferComplete = "Transfer complete";

        public static string TRXTypeEarned = "E";

        public static string GetEarnedThroughRPSLog(float ReceivedAmount)
        {
            return "Earned " + ReceivedAmount.ToString() + " phonies by playing rock paper scissors.";
        }

        public static string GetLostThroughRPSLog(float SetnAmount)
        {
            return "Lost " + SetnAmount.ToString() + " phonies by playing rock paper scissors.";
        }

        public static string TRXTypeGifted = "G";

        public static string GetPresentCollectedLog(string PresentType, string PresentAmount)
        {
            return "Collected a " + PresentType + " present of " + PresentAmount + " phonies.";
        }

        public static string TRXTypeLoanTaken = "LT";

        public static string GetLoanTakenLog(string LoanType, float LoanAmount)
        {
            return "Took a " + LoanType + " of " + LoanAmount.ToString() + " phonies ";
        }

        public static string TRXTypeLoanGiven = "LG";

        public static string GetLoanPaidLog(float ClearanceAmount, float AmountWithInterest)
        {
            return "Paid " + ClearanceAmount.ToString() + " phonies from loan of " + AmountWithInterest.ToString();
        }

        public static string PaymentCompleted = "Payment completed";
    }
}

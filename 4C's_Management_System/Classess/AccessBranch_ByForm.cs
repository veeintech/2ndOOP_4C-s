using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4C_s_Management_System.Classess
{
    internal class AccessBranch_ByForm
    {
        public static string BranchId { get; private set; } = string.Empty;
        public static string PaymentId { get; private set; } = string.Empty;

        public static bool x { get; set; } = false;

        public  static void CBCHECKED(bool cbbool)
        {
            x = cbbool;
        }

        // Store the branch after successful login
        public static void SetBranch(string branchId)
        {
            BranchId = branchId;
        }

        // Clear the branch when logging out
        public static void ClearSession()
        {
            BranchId = string.Empty;
        }

        public static bool IsLoggedIn =>
            !string.IsNullOrWhiteSpace(BranchId);

        public static void SetPayment(string Payment)
        {
            PaymentId = Payment;
        }

        public static void ClearPaymentID()
        {
            PaymentId = string.Empty;
        }
    }
}

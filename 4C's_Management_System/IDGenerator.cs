
using System;
using Microsoft.Data.SqlClient;

namespace _4C_s_Management_System
{
    internal static class IDGenerator
    {
        // Generate IDs for customers, tenants, reservations, users, etc.
        public static string GenerateNextID(
            string prefix,
            string branchId)
        {
            object? result = AppDB.Instance.Scalar(
                  "EXEC dbo.GenerateNextIDNumber @Prefix, @BranchId",
              new
                 {
                        Prefix = prefix,
                        BranchId = branchId
                    }
);

            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException(
                    "Unable to generate ID. Check the branch and sequence record."
                );
            }

            long number = Convert.ToInt64(result);

            // Extract numeric portion of BranchId.
            string[] parts = branchId.Split('-');

            if (parts.Length != 2 ||
                !long.TryParse(parts[1], out _))
            {
                throw new ArgumentException("Invalid Branch ID.");
            }

            string branchNumber = parts[1];

            return $"{prefix}-{branchNumber}-{number:D6}";
        }

        // Generate Branch IDs without the branch number portion.
        public static string GenerateNextBranchID()
        {
            object? result = AppDB.Instance.Scalar(
                "SELECT NEXT VALUE FOR dbo.BranchIDSequence;"
            );

            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException(
                    "Failed to generate Branch ID."
                );
            }

            long number = Convert.ToInt64(result);

            return $"BRN-{number:D6}";
        }
    }
}

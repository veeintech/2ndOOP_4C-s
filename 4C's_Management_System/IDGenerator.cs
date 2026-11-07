namespace _4C_s_Management_System
{
    internal static class IDGenerator
    {
        public static string GenerateID(string prefix, long number)
        {
            return $"{prefix}-{number:D5}";
        }

        public static string GenerateNextCustomerID(string prefix)
        {
            object? result = AppDB.Instance.Scalar(
                "SELECT NEXT VALUE FOR CustomerIdSequence");

            long number = Convert.ToInt64(result);

            return GenerateID(prefix, number);
        }
    }
}
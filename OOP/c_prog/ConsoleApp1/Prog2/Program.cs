using System;

class Program
{
    static void Main()
    {
        int[] numbers = { 12, 7, 25, 18, 30, 11, 42, 9, 16, 21, 8, 33, 50, 14, 5 };

        int evenCount = 0;
        int oddCount = 0;

        Console.Write("List of Even numbers: ");
        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] % 2 == 0)
            {
                Console.Write($"{numbers[i]} ");
                evenCount++;
            }
        }

        Console.Write("\n\nList of Odd numbers: ");
        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] % 2 != 0)
            {
                Console.Write($"{numbers[i]} ");
                oddCount++;
            }
        }

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine($"Total Even numbers: {evenCount}");
        Console.WriteLine($"Total Odd numbers: {oddCount}");
    }
}
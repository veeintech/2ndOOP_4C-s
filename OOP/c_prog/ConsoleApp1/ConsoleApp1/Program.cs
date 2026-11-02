using System;

class Program
{
    static void Main()
    {
        int[] numbers = { 25, 10, 45, 32, 18, 67, 5, 90, 41, 12 };

        int largest = numbers[0];
        int position = 0;

        for (int i = 1; i < numbers.Length; i++)
        {
            if (numbers[i] > largest)
            {
                largest = numbers[i];
                position = i;
            }
        }

        Console.WriteLine($"Largest number: {largest}" + largest);
        Console.WriteLine($"Position: {position}");
    }
}

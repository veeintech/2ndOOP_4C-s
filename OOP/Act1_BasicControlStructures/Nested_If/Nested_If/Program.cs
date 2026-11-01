using System;

class Program
{
    static void Main()
    {
        Console.Write("Enter a number: ");
        int number = Convert.ToInt32(Console.ReadLine());

        if (number >= 0)
        {
            if (number > 0)
            {
                Console.WriteLine("Positive");
            }
            else
            {
                Console.WriteLine("Zero");
            }
        }
        else
        {
            Console.WriteLine("Negative");
        }
        Console.WriteLine();

        Console.WriteLine("Programmed by: Calaluan, Venice Alces P.");
    }
}
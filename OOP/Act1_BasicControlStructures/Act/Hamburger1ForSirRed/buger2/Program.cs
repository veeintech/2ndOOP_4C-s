using System;
class Program
{
    static void Main()
    {
        Console.WriteLine("Type number: ");
        int n = int.Parse(Console.ReadLine());

        if  (n < 0)
        {
            Console.WriteLine("Negative");
        }
        else if (n > 0) 
        {
            Console.WriteLine("Positive");
        }
        else
        {
            Console.WriteLine("Zero");
        }
        Console.WriteLine();
    }
}
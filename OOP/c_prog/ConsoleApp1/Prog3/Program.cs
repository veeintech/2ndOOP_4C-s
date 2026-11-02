using System;

class Program
{
    static void Main()
    {
        bool vInput = false;

        while (!vInput)
        {
            try
            {
                Console.Write("Enter first number: ");
                double num1 = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter second number: ");
                double num2 = Convert.ToDouble(Console.ReadLine());

                double result = num1 / num2;

                if (num2 == 0)
                {
                    throw new DivideByZeroException();
                }

                Console.WriteLine($"Result: {result}");
                vInput = true;
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: Please enter valid numbers.");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Error: Cannot divide by zero.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }

            Console.WriteLine();
        }
    }
}
using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Stack<string> names = new Stack<string>();

        Console.Write("Enter Last Name: ");
        string lastName = Console.ReadLine();

        Console.Write("Enter Middle Name: ");
        string middleName = Console.ReadLine();

        Console.Write("Enter First Name: ");
        string firstName = Console.ReadLine();

        names.Push(lastName);
        names.Push(middleName);
        names.Push(firstName);

        // Display Stack
        Console.Write("\nUser Input Display: {0} {1} {2}", firstName, middleName, lastName);


    }
}


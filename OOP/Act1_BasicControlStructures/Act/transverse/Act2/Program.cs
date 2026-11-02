using System;

class Program
{
    static void Main()
    {
        Console.Write("Enter name: ");
        string name = Console.ReadLine();
        char[] arrVEN = name.ToCharArray();

        Console.Write("Enter a character: ");
        char insertMe = Convert.ToChar(Console.ReadLine());

        Console.Write("Enter position: ");
        int position = int.Parse(Console.ReadLine());

        int size = arrVEN.Length;

        Console.Write("Insert " + insertMe + " to position " + position + " ---> ");
        char[] VEN2 = new char[size + 1];

        for (int x = 0; x < VEN2.Length; x++)
        {
            if (x < position)
            {
                VEN2[x] = arrVEN[x];
            }
            else if (x == position)
            {
                VEN2[x] = insertMe;
            }
            else
            {
                VEN2[x] = arrVEN[x - 1];
            }
        }

        Console.Write("  NOW --->  ");

        for (int x = 0; x < VEN2.Length; x++)
        {
            Console.Write(" " + VEN2[x]);
        }
    }
}
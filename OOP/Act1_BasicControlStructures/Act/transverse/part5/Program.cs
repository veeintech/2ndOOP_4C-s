using System;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        char[] arrVEN = { 'C', 'A', 'L', 'A', 'L', 'U', 'A', 'N' };
        char insertMe = 'A';
        int position = 6;

        Console.Write("Update Element at position " + position + " with Element " + insertMe + " ---->  ");

        for (int x = 0; x < arrVEN.Length; x++)
        {
            if (x == position)
            {
                arrVEN[x] = insertMe;
            }
        }

        Console.Write("  NOW --->  ");

        for (int x = 0; x < arrVEN.Length; x++)
        {
            Console.Write(" " + arrVEN[x]);
        }
    }
}
using System;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        char[] VEN = { 'C', 'A', 'L', 'A', 'L', 'U', 'A', 'N' };
        int size = VEN.Length;
        int count = 0;

        Console.Write("Input an element you are looking for: ");
        char SearchforMe = Convert.ToChar(Console.ReadLine());

        Console.Write("Search for Element " + SearchforMe + " Element---- > ");

        for (int x = 0; x < VEN.Length; x++)
        {
            if (VEN[x] == SearchforMe)
            {
                count = ++count;
            }
        }

        Console.WriteLine("I found " + count + " " + SearchforMe + "!");


    }
}
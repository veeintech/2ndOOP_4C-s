using System;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        Console.WriteLine("Foreach Loop Output:");
        string[] names = { "Jyrha", "Mae", "Jheansen" };

        foreach (string name in names)
        {
            Console.Write(name + " ");
        }
        Console.WriteLine();

        Console.WriteLine("Programmed by: Calaluan, Venice Alces P.");
    }
}
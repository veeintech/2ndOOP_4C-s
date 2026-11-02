using System;

class Program
{
    static void Main()
    {
        char[] arrVENICE = { 'V', 'E', 'N', 'I', 'C', 'E' };
        int size = arrVENICE.Length;
        int DeletePos = 4;

        Console.Write("Delete Number " + DeletePos + " Element ---- >  ");

        for (int j = DeletePos; j < size; j++)
        {
            arrVENICE[j - 1] = arrVENICE[j];
        }

        Console.Write("  NOW---> ");

        for (int x = 0; x < arrVENICE.Length - 1; x++)
        {
            Console.Write(" " + arrVENICE [x]);
        }
    }
}
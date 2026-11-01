using System;

class Program
{
    static void Main()
    {
        char[] arrVENICE = { 'V', 'E', 'N', 'I', 'C', 'E' };
        int size = arrVENICE.Length;
        char insertMe = 'I';
        int position = 4;

        Console.Write("Insert " + insertMe + " to position " + position + "--->");

        char[] arrVENICE2 = new char[size + 1];

        for (int x = 0; x < arrVENICE2.Length; x++)

        {
            if (x < position ) { arrVENICE2[x] = arrVENICE[x]; }

            else if (x == position)
            {
                arrVENICE2[x] = insertMe;

            }

            else { arrVENICE2[x] = arrVENICE[x - 1]; }
        }

        Console.Write(" NOW--->");

        for (int x = 0; x < arrVENICE2.Length; x++)

        {

            Console.Write(" " + arrVENICE2[x]);

        }

    }
}
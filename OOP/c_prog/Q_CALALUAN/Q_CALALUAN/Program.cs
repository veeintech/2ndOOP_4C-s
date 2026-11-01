using System;

class Program
{
    static void Main()
    {
        Queue<int> AkingQueue = new Queue<int>();

        AkingQueue.Enqueue(1);
        AkingQueue.Enqueue(2);
        AkingQueue.Enqueue(3);
        AkingQueue.Enqueue(4);

        foreach (int VALUE in AkingQueue)
        {
            Console.Write($"| {VALUE} |");
        }

        Console.WriteLine($"\nFRONT VALUE: {AkingQueue.Peek()}");

        Console.WriteLine($"I REMOVED: {AkingQueue.Dequeue()}");

        Console.WriteLine($"FRONT VALUE NOW IS: {AkingQueue.Peek()}");

        int unahan = AkingQueue.Dequeue();

        Console.WriteLine($"Anong inalis ko?: {unahan}");

        AkingQueue.Enqueue(30);

        foreach (int VALUE in AkingQueue)
        {
            Console.Write($"| {VALUE} |");
        }
    }
}
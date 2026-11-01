using System;
using System.Collections.Generic;

class CollectionsStack
{
    static void Main()
    {
        Stack<int> stack = new Stack<int>();

        stack.Push(10);
        stack.Push(20);
        stack.Push(30);
        Console.WriteLine($"Stack after pushes (top -> bottom): {string.Join(", ", stack)}");

        Console.WriteLine($"Top element (Peek): {stack.Peek()}");

        
        Console.WriteLine($"Popped: {stack.Pop()}");
        Console.WriteLine($"Stack after pop: {string.Join(", ", stack)}");

        
        Console.WriteLine($"Count: { stack.Count}");
        Console.WriteLine($"Contains 10? {stack.Contains(10)}");

        
        stack.Pop();
        stack.Pop();
        Console.WriteLine($"Is empty after popping all? {(stack.Count == 0)}");

        try
        {
            stack.Pop(); 
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("Cannot pop: the stack is empty (InvalidOperationException).");
        }

        
        if (!stack.TryPop(out int value))
        {
            Console.WriteLine("TryPop returned false: stack is empty.");
        }

        
        stack.Push(1);
        stack.Push(2);
        stack.Clear();
        Console.WriteLine($"Count after Clear: {stack.Count}");
    }
}

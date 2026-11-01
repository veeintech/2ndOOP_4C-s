using System;

class MyStack
{
    private int[] data;
    private int top;
    private int capacity;

    public MyStack(int capacity)
    {
        this.capacity = capacity;
        data = new int[capacity];
        top = -1;
    }

    public bool IsEmpty => top == -1;
    public bool IsFull => top == capacity - 1;
    public int Size => top + 1;

    public bool Push(int value)
    {
        if (IsFull)
        {
            Console.WriteLine($"Stack Overflow: cannot push {value}");
            return false;
        }
        data[++top] = value;
        Console.WriteLine($"{value} pushed.");
        return true;
    }

    public bool Pop(out int value)
    {
        if (IsEmpty)
        {
            Console.WriteLine("Stack Underflow: nothing to pop.");
            value = 0;
            return false;
        }
        value = data[top--];
        return true;
    }

    public bool Peek(out int value)
    {
        if (IsEmpty)
        {
            Console.WriteLine("Stack is empty: nothing to peek.");
            value = 0;
            return false;
        }
        value = data[top];
        return true;
    }

    public void Display()
    {
        if (IsEmpty)
        {
            Console.WriteLine("Stack is empty.");
            return;
        }
        Console.WriteLine("Stack (top -> bottom):");
        for (int i = top; i >= 0; i--)
        {
            Console.WriteLine("  " + data[i]);
        }
    }
}

class UserDefinedStack
{
   
    static int ReadInt(string prompt)
    {
        int result;
        Console.Write(prompt);
        while (!int.TryParse(Console.ReadLine(), out result))
        {
            Console.Write("Invalid number, try again: ");
        }
        return result;
    }

    static void Main()
    {
        MyStack stack = new MyStack(5);
        int choice;

        do
        {
            Console.WriteLine("\n=== STACK MENU ===");
            Console.WriteLine("1. Push");
            Console.WriteLine("2. Pop");
            Console.WriteLine("3. Peek");
            Console.WriteLine("4. Display");
            Console.WriteLine("5. Size");
            Console.WriteLine("6. Check if empty / full");
            Console.WriteLine("0. Exit");
            choice = ReadInt("Choose: ");

            switch (choice)
            {
                case 1:
                    stack.Push(ReadInt("Enter value: "));
                    break;
                case 2:
                    if (stack.Pop(out int popped))
                        Console.WriteLine($"Popped: {popped}");
                    break;
                case 3:
                    if (stack.Peek(out int top))
                        Console.WriteLine($"Top: {top}");
                    break;
                case 4:
                    stack.Display();
                    break;
                case 5:
                    Console.WriteLine($"Size: {stack.Size}");
                    break;
                case 6:
                    Console.WriteLine($"Empty? {stack.IsEmpty} | Full? {stack.IsFull}");
                    break;
                case 0:
                    Console.WriteLine("Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        } 
        while (choice != 0);
    }
}

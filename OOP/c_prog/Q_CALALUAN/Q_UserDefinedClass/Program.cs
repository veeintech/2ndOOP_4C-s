using System;

class mhacel
{
    private string[] items;
    private int front, rear, size, capacity;

    public mhacel(int capacity)
    {
        this.capacity = capacity;
        items = new string[capacity];
        front = -1;
        rear = -1;
        size = 0;
    }

    public void Enqueue(string value)
    {
        if (rear == capacity - 1)
        {
            Console.WriteLine($"\nQueue Overflow! Cannot insert {value}");
            return;
        }

        if (front == -1)
            front = 0;

        items[++rear] = value;
        size++;

        Console.WriteLine($"\n{value} inserted into the queue.");
    }

    public void Dequeue()
    {
        if (front == -1)
        {
            Console.WriteLine("\nQueue Underflow! Queue is empty.");
            return;
        }
        
        Console.WriteLine("\n" + items[front] + " removed from the queue.");

        if (front == rear)
            front = rear = -1;
        else
            front++;

        size--;
    }

    public void Peek()
    {
        if (front == -1)
            Console.WriteLine("\nQueue is empty.");
        else
            Console.WriteLine($"\nFront element:  " + items[front]);
    }

    public void Display()
    {
        if (front == -1)
        {
            Console.WriteLine("\nQueue is empty.");
            return;
        }

        Console.Write("\nQueue elements:  ");
        for (int i = front; i <= rear; i++)
        {
            Console.Write(items[i] + "  ");
        }
        Console.WriteLine();
    }

    public bool IsFull()
    {
        return size == capacity;
    }

    public bool IsEmpty()
    {
        return size == 0;
    }
}

class Program
{
    static void Main(string[] args)
    {
        mhacel horimiya = new mhacel(5);

        horimiya.Enqueue("Miyamura");
        horimiya.Enqueue("Hori");
        horimiya.Enqueue("Sengoku");

        horimiya.Display();
        horimiya.Peek();

        horimiya.Dequeue();
        horimiya.Display();

        if (!horimiya.IsFull())
        {
            Console.Write("\nEnter a string to enqueue: ");
            string val = Console.ReadLine();
            horimiya.Enqueue(val);
        }

        if (!horimiya.IsEmpty())
        {
            horimiya.Dequeue();
        }
    }
}
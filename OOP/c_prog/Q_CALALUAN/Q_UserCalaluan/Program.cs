using System;

namespace Queue_Calaluan
{
    internal class Program
    {
        public class Maricel
        {
            private string[] items;
            private int front, rear, size, capacity;

            public Maricel(int capacity)
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
                {
                    front = 0;
                }

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

                Console.WriteLine("\n" + items[front] + "removed from the queue.");

                if (front == rear)
                {
                    front = rear = -1;
                }
                else
                {
                    front++;
                }

                size--;
            }

            public void Peek()
            {
                if (front == -1)
                {
                    Console.WriteLine("\nQueue is empty.");
                }
                else
                {
                    Console.WriteLine($"\nFront element:  {items[front]}");
                }
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
                    Console.Write($"{items[i]}  ");
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

        class Queue
        {
            static void Main(string[] args)
            {
                Maricel horimiya = new Maricel(5);

                horimiya.Enqueue("Miyamura");
                horimiya.Enqueue("Hori");
                horimiya.Enqueue("Sengoku");
                Console.WriteLine();

                horimiya.Display();
                Console.WriteLine();

                horimiya.Peek();
                Console.WriteLine();

                horimiya.Dequeue();
                horimiya.Display();
                Console.WriteLine();

                horimiya.Dequeue();
                horimiya.Dequeue();
                horimiya.Dequeue();

                if (horimiya.IsFull())
                {
                    Console.WriteLine("\nQueue Overflow! Cannot enqueue,full capacity.");
                }
                else
                {
                    Console.Write("\nEnter a item: ");
                    string it = Console.ReadLine();
                    horimiya.Enqueue(it);
                }

                if (horimiya.IsEmpty())
                {
                    Console.WriteLine("\nQueue Underflow! No element to dequeue.");
                }
                else
                {
                    horimiya.Dequeue();
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queue_Malapote
{
    internal class Program
    {
        public class Neriza
        {
            private string[] items;
            private int front, rear, size, capacity;

            public Neriza(int capacity)
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
                    Console.WriteLine($"Queue Overflow! Cannot insert [{value}]");
                    return;
                }

                if (front == -1)
                {
                    front = 0;
                }

                items[++rear] = value;
                size++;

                Console.WriteLine($"[{value}] inserted into the queue.");
            }

            public void Dequeue()
            {
                if (front == -1)
                {
                    Console.WriteLine("Queue Underflow! Queue is empty.");
                    return;
                }

                Console.WriteLine($"[{items[front]}] removed from the queue.");

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
                    Console.WriteLine("Queue is empty.");
                }
                else
                {
                    Console.WriteLine($"Front element: [{items[front]}]");
                }
            }

            public void Display()
            {
                if (front == -1)
                {
                    Console.WriteLine("Queue is empty.");
                    return;
                }

                Console.Write("Queue elements: ");

                for (int i = front; i <= rear; i++)
                {
                    Console.Write($"[{items[i]}] || ");
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
                Neriza aot = new Neriza(5);

                aot.Enqueue("Attack on Titan");
                aot.Enqueue("Eren");
                aot.Enqueue("Levi");
                Console.WriteLine();

                aot.Display();
                Console.WriteLine();

                aot.Peek();
                Console.WriteLine();

                aot.Dequeue();
                aot.Display();
                Console.WriteLine();

                aot.Dequeue();
                aot.Dequeue();
                aot.Dequeue();

                if (aot.IsFull())
                {
                    Console.WriteLine("\nQueue Overflow! Cannot enqueue,full capacity.");
                }
                else
                {
                    Console.Write("\nEnter your favorite character: ");
                    string mc = Console.ReadLine();
                    aot.Enqueue(mc);
                }

                if (aot.IsEmpty())
                {
                    Console.WriteLine("\nQueue Underflow! No element to dequeue.");
                }
                else
                {
                    aot.Dequeue();
                }
            }
        }
    }
}

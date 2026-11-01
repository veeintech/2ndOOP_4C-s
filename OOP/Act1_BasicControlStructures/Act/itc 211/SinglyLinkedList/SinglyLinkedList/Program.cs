using System;

namespace Singly_List
{
    class Node
    {
        public int Data;
        public Node Next;

        public Node(int data)
        {
            Data = data;
            Next = null;
        }
    }

    public class Listahannimama
    {
        private Node head;
        public void InsertsaUNA(int data)
        {
            Node newCalaluan = new Node(data);
            newCalaluan.Next = head;
            head = newCalaluan;
        }
        public void InsertsaHULI(int data)
        {
            Node newCalaluan = new Node(data);
            if (head == null)
            {
                head = newCalaluan;
                return;
            }

            Node temp = head;
            while (temp.Next != null)
            {
                temp = temp.Next;
            }
            temp.Next = newCalaluan;
        }
        public void DeletesaUNA()
        {
            if (head == null)
            {
                Console.WriteLine("List is empty. No data to delete");
                return;
            }
            head = head.Next;
        }

        public void DeletesaHULI()
        {
            if (head == null)
            {
                Console.WriteLine("List is empty. Nothing to delete.");
                return;
            }

            if (head.Next == null)
            {
                head = null;
                return;
            }

            Node temp = head;
            while (temp.Next.Next != null)
            {
                temp = temp.Next;
            }
            temp.Next = null;
        }
        public bool Search(int value)
        {
            Node temp = head;
            while (temp != null)
            {
                if (temp.Data == value)
                {
                    return true;
                }
                temp = temp.Next;
            }
            return false;
        }
        public void Display()
        {
            Node temp = head;
            while (temp != null)
            {
                Console.Write(temp.Data + " || ");
                temp = temp.Next;
            }

            Console.WriteLine("null");
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, this is Linked List!");
            Listahannimama Venice = new Listahannimama();

            Venice.InsertsaHULI(10);
            Venice.InsertsaHULI(20);
            Venice.InsertsaHULI(30);
            Venice.InsertsaUNA(5);
            Venice.InsertsaUNA(10);
            Venice.InsertsaUNA(15);
            Venice.DeletesaHULI();
            Venice.DeletesaUNA();

            Venice.Display();

            Console.Write("\n\nEnter value to search: ");
            int searchValue = int.Parse(Console.ReadLine());

            bool found = Venice.Search(searchValue);
            if (found)
                Console.WriteLine($"{searchValue} value found!");
            else
                Console.WriteLine($"{searchValue} value does not found!");

            Console.ReadKey();
        }
    }
}
using System;

namespace List
{
    public class Node
    {
        public int Data; //dito i-store data
        public Node Next; //reference sa next node

        public Node(int data)
        {
            Data = data;
            Next = null;
        }
    }
    public class SinglyLinkedList
    {
        private Node head;

        public void InsertsaUNA(int data)
        {
            Node newNode = new Node(data);
            newNode.Next = head;
            head = newNode;
        }

        public void InsertsaHuli(int data)
        {
            Node newNode = new Node(data);
            if (head == null)
            {
                head = newNode;
                return;
            }

            Node temp = head;
            while (temp.Next != null)
            {
                temp = temp.Next;
            }
            temp.Next = newNode;
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
        static void Main()
        {
            Console.WriteLine("Hello, this is Linked List");
            SinglyLinkedList list = new SinglyLinkedList();

            list.InsertsaHuli(10);
            list.InsertsaHuli(20);
            list.InsertsaUNA(30);
            list.InsertsaUNA(40);

            list.Display();
            Console.ReadKey();
        }
    }
}
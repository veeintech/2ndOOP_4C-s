using System;

namespace doubly
{
    public class Nicka
    {
        public int Data;
        public Nicka Next;
        public Nicka Previous;
        public Nicka(int data)
        {
            Data = data;
            Next = null;
            Previous = null;
        }
    }
    public class DoublyListahanniate
    {
        private Nicka head;
        public void InsertsaUna(int data)
        {
            Nicka newCalaluan = new Nicka(data);
            if (head == null)
            {
                head = newCalaluan;
                return;
            }
            newCalaluan.Next = head;
            head.Previous = newCalaluan;
            head = newCalaluan;
        }
        public void InsertsaHuli(int data)
        {
            Nicka newCalaluan = new Nicka(data);
            if (head == null)
            {
                head = newCalaluan;
                return;
            }
            Nicka temp = head;
            while (temp.Next != null)
            {
                temp = temp.Next;
            }
            temp.Next = newCalaluan;
            newCalaluan.Previous = temp;
        }
        public void DeletesaUNA()
        {
            if (head == null)
            {
                Console.WriteLine("Empty");
                return;
            }
            head = head.Next;
            if (head != null)
                head.Previous = null;
        }
        public void DeletesaHULI()
        {
            if (head == null)
            {
                Console.WriteLine("List is empty.");
                return;
            }
            if (head.Next == null)
            {
                head = null;
                return;
            }
            Nicka temp = head;
            while (temp.Next != null)
            {
                temp = temp.Next;
            }
            temp.Previous.Next = null;
        }

        public void ForwardSearch(int value)
        {
            Nicka temp = head;

            while (temp != null)
            {
                if (temp.Data == value)
                {
                    Console.WriteLine($"{value} found using Forward Search!");
                    return;
                }
                temp = temp.Next;
            }

            Console.WriteLine($"{value} not found.");
        }

        public void ReverseSearch(int value)
        {
            if (head == null)
            {
                Console.WriteLine("List is empty.");
                return;
            }

            Nicka temp = head;

            while (temp.Next != null)
            {
                temp = temp.Next;
            }

            while (temp != null)
            {
                if (temp.Data == value)
                {
                    Console.WriteLine($"{value} found using Reverse Search!");
                    return;
                }
                temp = temp.Previous;
            }

            Console.WriteLine($"{value} not found.");
        }

        public void DisplayForward()
        {
            Nicka temp = head;
            while (temp != null)
            {
                Console.Write(temp.Data + " <-> ");
                temp = temp.Next;
            }
            Console.WriteLine("null");
        }
        public void DisplayBackward()
        {
            if (head == null)
            {
                Console.WriteLine("List is empty.");
                return;
            }

            Nicka temp = head;
            while (temp.Next != null)
            {
                temp = temp.Next;
            }

            while (temp != null)
            {
                Console.Write(temp.Data + " <-> ");
                temp = temp.Previous;
            }
            Console.WriteLine("null");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            DoublyListahanniate Chummy = new DoublyListahanniate();

            Chummy.InsertsaHuli(11);
            Chummy.InsertsaHuli(2002);
            Chummy.InsertsaHuli(2007);
            Chummy.InsertsaUna(21);

            Console.WriteLine("Traverse Forward: ");
            Chummy.DisplayForward();

            Console.WriteLine("\nTraverse Backward: ");
            Chummy.DisplayBackward();

            Chummy.DeletesaUNA();
            Console.WriteLine("\nAfter deleting front: ");
            Chummy.DisplayForward();

            Chummy.DeletesaHULI();
            Console.WriteLine("\nAfter deleting end: ");
            Chummy.DisplayForward();

            Console.Write("\nEnter a number to search: ");
            int searchNum = int.Parse(Console.ReadLine());

            Console.Write("Search direction? (Forward / Reverse): ");
            string searchniChum = Console.ReadLine().ToUpper();

            if (searchniChum == "FORWARD")
                Chummy.ForwardSearch(searchNum);
            else
                Chummy.ReverseSearch(searchNum);
        }
    }
}
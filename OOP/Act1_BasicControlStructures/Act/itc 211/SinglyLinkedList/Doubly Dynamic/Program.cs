namespace Doubly_Dynamic
{
    internal class Program
    {
        class Nika
        {
            public int Data;
            public Nika Next;
            public Nika Prev;

            public Nika(int data)
            {
                Data = data;
                Next = null;
                Prev = null;
            }
        }

        public class mamamaror
        {
            private Nika head;
            private Nika tail;

            // INSERT HEAD
            public void InsertFirst(int data)
            {
                Nika newNode = new Nika(data);

                if (head == null)
                {
                    head = tail = newNode;
                }
                else
                {
                    newNode.Next = head;
                    head.Prev = newNode;
                    head = newNode;
                }
            }

            // INSERT TAIL
            public void InsertLast(int data)
            {
                Nika newNode = new Nika(data);

                if (tail == null)
                {
                    head = tail = newNode;
                }
                else
                {
                    tail.Next = newNode;
                    newNode.Prev = tail;
                    tail = newNode;
                }
            }

            // DELETE HEAD
            public void DeleteFirst()
            {
                if (head == null)
                {
                    Console.WriteLine("List is Empty.");
                    return;
                }

                if (head == tail)
                {
                    head = tail = null;
                }
                else
                {
                    head = head.Next;
                    head.Prev = null;
                }

                Console.WriteLine("Deleted!");
            }

            // DELETE TAIL
            public void DeleteLast()
            {
                if (tail == null)
                {
                    Console.WriteLine("List is Empty.");
                    return;
                }

                if (head == tail)
                {
                    head = tail = null;
                }
                else
                {
                    tail = tail.Prev;
                    tail.Next = null;
                }

                Console.WriteLine("Deleted!");
            }

            // SEARCH
            public void Search(int data)
            {
                Nika temp = head;
                int position = 1;

                while (temp != null)
                {
                    if (temp.Data == data)
                    {
                        Console.WriteLine("Found!");
                        Console.WriteLine("Element position: " + position);
                        return;
                    }

                    temp = temp.Next;

                    Console.WriteLine();
                }

            }
        }
    }
}
            
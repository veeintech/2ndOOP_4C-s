namespace DoublyDynamic
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

        public class Doublynimama
        {
            private Nika head;
            private Nika tail;

            public char chosen;


            // INSERT una
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

            // INSERT huli
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

            // DELETE first
            public void DeleteFirst()
            {
                if (head == null)
                {
                    Console.WriteLine("List is Empty.");
                    return;
                }

                if (head == tail)
                {
                    head = null;
                    tail = null;
                }
                else
                {
                    head = head.Next;
                    head.Prev = null;
                }
            }

            // DELETE LAST
            public void DeleteLast()
            {
                if (tail == null)
                {
                    Console.WriteLine("List is Empty.");
                    return;
                }

                if (head == tail)
                {
                    head = null;
                    tail = null;
                }
                else
                {
                    tail = tail.Prev;
                    tail.Next = null;
                }
            }

            // SEARCH forward
            public void Search(int data)
            {
                Nika temp = head;
                int index = 1;

                while (temp != null)
                {
                    if (temp.Data == data)
                    {
                        Console.WriteLine("Found!");
                        Console.WriteLine("Element position: " + index + "\n");
                        return;
                    }

                    temp = temp.Next;
                    index++;
                }

                Console.WriteLine("\nElement not found.\n");
            }

            // SEARCH reverse
            public void ReverseSearch(int data)
            {
                Nika temp = tail;
                int index = 1;

                while (temp != null)
                {
                    if (temp.Data == data)
                    {
                        Console.WriteLine("Found!");
                        Console.WriteLine($"Element position from reverse: {index} \n");
                        return;
                    }

                    temp = temp.Prev;
                    index++;
                }

                Console.WriteLine("\nElement not found.\n");
            }

            // DISPLAY FORWARD
            public void Display()
            {
                Console.Write("\nCurrent Linked-List: ");
                Nika temp = head;

                while (temp != null)
                {
                    Console.Write(temp.Data + " <--> ");
                    temp = temp.Next;
                }

                Console.WriteLine("null\n");
            }

            // DISPLAY REVERSE
            public void ReverseDisplay()
            {
                Console.Write("\nCurrent List (Reverse): ");
                Nika temp = tail;

                while (temp != null)
                {
                    Console.Write(temp.Data + " <--> ");
                    temp = temp.Prev;
                }

                Console.WriteLine("null\n");
            }

            // ASK OPERATION
            public void AskOperation()
            {
                bool x = true;

                while (x)
                {
                    Console.WriteLine(
                        "\nWhat operation would you like to choose? " +
                        "\n(insert (i) / delete (d) / search (s) / traverse (t)): ");

                    chosen = char.Parse(Console.ReadLine().ToLower());

                    if (chosen != 'i' && chosen != 'd' && chosen != 's' && chosen != 't')
                    {
                        Console.WriteLine("\nInvalid Character!\n");
                        continue;
                    }

                    else if ((chosen == 'd' || chosen == 's') && head == null)
                    {
                        Console.WriteLine("\nOperation not available!\n");
                        continue;
                    }

                    else if (chosen == 'i')
                    {
                        InsertOperation();
                    }

                    else if (chosen == 's')
                    {
                        SearchOperation();
                    }

                    else if (chosen == 'd')
                    {
                        DeleteOperation();
                    }

                    else if (chosen == 't')
                    {
                        Display();
                        continue;
                    }
                }
            }

            // INSERT OPERATION
            public void InsertOperation()
            {
                int index;

                if (chosen == 'i')
                {
                    while (true)
                    {
                        Console.Write("\nHow many elements do you want to insert?: ");

                        if (int.TryParse(Console.ReadLine(), out index) && index > 0)
                        {
                            for (int i = 1; i <= index; i++)
                            {
                                Console.Write($"Enter Number {i}: ");
                                int number = int.Parse(Console.ReadLine());

                                char insert;

                                while (true)
                                {
                                    Console.Write("Where to insert (una(u) / huli(h))?: ");
                                    insert = char.Parse(Console.ReadLine().ToLower());

                                    if (insert != 'u' && insert != 'h')
                                    {
                                        Console.WriteLine("Invalid Character!");
                                        continue;
                                    }
                                    break;
                                }

                                if (insert == 'u')
                                {
                                    InsertFirst(number);
                                }
                                else
                                {
                                    InsertLast(number);
                                }
                            }

                            Display();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid number!");
                        }
                    }
                }

                AskOperation();
            }

            // SEARCH OPERATION
            public void SearchOperation()
            {
                int number;

                while (true)
                {
                    Console.Write("\nWhat number do you want to search?: ");

                    if (int.TryParse(Console.ReadLine(), out number) && number > 0)
                    {
                        char direction;
                        while (true)
                        {
                            Console.Write("Search direction (forward(f) / reverse(r)): ");
                            direction = char.Parse(Console.ReadLine().ToLower());

                            if (direction != 'f' && direction != 'r')
                            {
                                Console.WriteLine("Invalid Character!");
                                continue;
                            }

                            break;
                        }

                        if (direction == 'f')
                        {
                            Search(number);
                        }
                        else
                        {
                            ReverseSearch(number);
                        }

                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid number!");
                    }
                }

                AskOperation();
            }

            // DELETE OPERATION
            public void DeleteOperation()
            {
                char delete;

                while (true)
                {
                    Console.Write("Where to delete (una(u) / huli(h))?: ");
                    delete = char.Parse(Console.ReadLine().ToLower());

                    if (delete != 'u' && delete != 'h')
                    {
                        Console.WriteLine("Invalid Character!");
                        continue;
                    }
                    break;
                }

                if (delete == 'u')
                {
                    DeleteFirst();
                    Display();
                }
                else
                {
                    DeleteLast();
                    Display();
                }
                Display();
                AskOperation();
            }

            public void Menu()
            {
                Console.Write("\nChoice: ");
                int menu = int.Parse(Console.ReadLine());

                if (menu == 1)
                {
                    Console.Write("\nNode: ");
                    int number = int.Parse(Console.ReadLine());
                    InsertFirst(number);
                    Menu();
                }

                else if (menu == 2)
                {
                    Console.Write("\nNode: ");
                    int number = int.Parse(Console.ReadLine());
                    InsertLast(number);
                    Menu();
                }
                else if (menu == 3)
                {
                    DeleteFirst();
                    Console.WriteLine("Deleted! Done na bes! ");
                    Menu();
                }
                else if (menu == 4)
                {
                    DeleteLast();
                    Console.Write("Delted! Done na bes! ");
                    Menu();
                }
                else if (menu == 5)
                {
                    Console.Write("Enter Number to Search: ");
                    int number = int.Parse(Console.ReadLine());
                    Search(number);
                    Menu();
                }
                else if (menu == 6) 
                { 
                    Display(); Menu(); 
                }
                else if (menu == 7)
                {
                    Console.WriteLine("Program Ended! Finish na!");
                    Environment.Exit(0);
                }
            }
        }


        static void Main(string[] args)
        {
            Doublynimama list = new Doublynimama();

            Console.Write("How many nodes?: ");
            int nodes = int.Parse(Console.ReadLine());

            Console.Write("\n ===== Menu: ===== \n" +
               "1. Insert (Head) \n" +
               "2. Insert (Tail) \n" +
               "3. Delete (Head) \n" +
               "4. Delete (Tail) \n" +
               "5. Search \n" +
               "6. Display All\n" +
               "7. Exit\n");

            for (int i = 1; i <= nodes; i++)
            {
                Console.Write($"\nEnter Node {i}: ");
                int number = int.Parse(Console.ReadLine());

                int option;

                while (true)
                {
                    Console.Write("Select Option: ");
                    option = int.Parse(Console.ReadLine());

                    if (option >= 1 && option <= 7)
                        break;

                    Console.WriteLine("Invalid Option!");
                }

                switch (option)
                {
                    case 1:
                        list.InsertFirst(number);
                        break;

                    case 2:
                        list.InsertLast(number);
                        break;

                    case 3:
                        list.DeleteFirst();
                        break;

                    case 4:
                        list.DeleteLast();
                        break;

                    case 5:
                        list.Search(number);
                        break;

                    case 6:
                        list.Display();
                        break;
                    case 7:
                        Console.WriteLine("Program Ended! Finish na!");
                        Environment.Exit(0);
                        break;
                }
            }

            Console.WriteLine("\nAll iterations completed!");
            list.Display();
            list.Menu();
        }
    }
}
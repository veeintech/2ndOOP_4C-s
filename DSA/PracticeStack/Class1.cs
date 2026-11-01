namespace PracticeSTack
{
    class user
    {
        public int[] arraymo = new int[10];
        public int top = -1;

        public bool IsEmpty()
        {
            return top == -1;
        }

        public bool IsFull()
        {
            return top == arraymo.Length - 1;
        }

        public void push(int data)
        {
            if (IsFull())
            {
                Console.WriteLine("Stack is full!");
                return;
            }

            top++;
            arraymo[top] = data;
            //Console.WriteLine($"{data} has been added to the stack");
        }

        public void pop()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Stack is Empty!");
                return;
            }

            Console.WriteLine($"{arraymo[top]} has been removed to the stack");
            top--;
        }

        public void peek()
        {
            if (IsEmpty())
            {
                Console.WriteLine("\nStack is Empty!");
                return;
            }

            Console.WriteLine($"\nTOP of the stack: {arraymo[top]}");
        }

        public void count()
        {
            if (IsEmpty())
            {
                Console.WriteLine("\nStack is Empty!");
                return;
            }

            Console.WriteLine($"\nCount: {top + 1}");
        }

        public void Display()
        {
            if (IsEmpty())
            {
                Console.WriteLine("\nStack is Empty!");
                return;
            }
            Console.Write("\nYour Stack: ");
            int temp = top;
            while (temp != -1)
            {
                Console.Write($"\n[{arraymo[temp]}] ");
                temp--;
            }
            Console.Write("\n");
        }

        public void Clear()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Stack is already Empty!");
                return;
            }
            top = -1;
            Console.WriteLine("Cleared!");
        }
        public void Contains(int data)
        {
            if (IsEmpty())
            {
                Console.WriteLine("Stack is already Empty!");
                return;
            }
            int temp = top;
            while (temp != -1)
            {
                if (arraymo[temp] == data)
                {
                    Console.WriteLine($"The stack contains {data} at index {temp}");
                    return;
                }
                temp--;
            }
            Console.WriteLine($"The stack do not contains {data}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            user u = new user();
            Console.Write("Pushed ==> ");
            int x = int.Parse(Console.ReadLine());
            u.push(x);
            Console.Write("Pushed ==> ");
            int i = int.Parse(Console.ReadLine());
            u.push(i);
            Console.Write("Pushed ==> ");
            int k = int.Parse(Console.ReadLine());
            u.push(k);
            u.pop();
            Console.Write("Pushed ==> ");
            int l = int.Parse(Console.ReadLine());
            u.push(l);
            u.pop();
            u.Display();
        }
    }
}
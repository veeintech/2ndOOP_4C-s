namespace PracticeSTack_Act1
{
    class stack
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

        public void push()
        {
            if (IsFull())
            {
                Console.WriteLine("Stack is full!");
                return;
            }

            top++;
            Console.Write("Pushed ==> ");
            int n = int.Parse(Console.ReadLine());
            arraymo[top] = n;
        }

        public void display()
        {
            if (IsEmpty())
            {
                Console.WriteLine("\nStack is Empty!");
                return;
            }
            Console.Write("Stack elements: ");
            int temp = top;
            while (temp != -1)
            {
                Console.Write($"\n| {arraymo[temp]} |");
                temp--;
            }
            Console.Write("\n");
        }

        public void pop()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Stack is Empty!");
                return;
            }

            Console.WriteLine($"{arraymo[top]} popped from stack.");
            top--;
        }

        public void peek()
        {
            if (IsEmpty())
            {
                Console.WriteLine("\nStack is Empty!");
                return;
            }

            Console.WriteLine($"\nStack elements: {arraymo[top]}");
        }

        internal class Program
        {
            static void Main(string[] args)
            {
                stack u = new stack();

                u.push();
                u.push();
                u.push();
                u.pop();
                u.push();
                u.pop();
                u.display();
            }
        }
    }
}
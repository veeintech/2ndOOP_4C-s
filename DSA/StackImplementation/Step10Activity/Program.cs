namespace Step10Activity
{
    class stack
    {
        public double[] arraymo;
        public int top = -1;

        public stack(int size)
        {
            arraymo = new double[size];
        }

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
            Console.Write($"Subject {top + 1}: ");
            double n = double.Parse(Console.ReadLine());
            arraymo[top] = n;
        }

        public void display()
        {
            if (IsEmpty())
            {
                Console.WriteLine("\nStack is Empty!");
                return;
            }
            Console.Write("\nStack of Grade:");
            int temp = top;
            while (temp != -1)
            {
                Console.Write($"\n|{arraymo[temp]:0.0}|");
                temp--;
            }
            Console.Write("\n");
        }

        public void average()
        {
            if (IsEmpty())
            {
                Console.WriteLine("\nStack is Empty!");
                return;
            }

            double sum = 0;
            int temp = top;
            while (temp != -1)
            {
                sum += arraymo[temp];
                temp--;
            }

            Console.WriteLine($"Midterm Grade: {(int)(sum / (top + 1))}");
        }

        internal class Program
        {
            static void Main(string[] args)
            {
                Console.Write("Name: ");
                string nm = Console.ReadLine();

                int n = 3;

                stack u = new stack(n);

                for (int i = 0; i < n; i++)
                {
                    u.push();
                }

                u.display();
                u.average();
            }
        }
    }
}
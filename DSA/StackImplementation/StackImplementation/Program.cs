namespace StackImplemnet
{
    class StackIm
    {
        private int[] Arrayko;
        private int Ibabaw, max;
    }

    public StackIm(int size)
        {
            Arrayko = new int[size];
            max = size;
            Ibabaw = -1;
        }
        public void Push(int valu)
        {
            if (Ibabaw == max - 1)
            {
                Console.WriteLine("Apaw na bes! is empty"
            }
        }

        public void Display()
        {
            if (Ibabaw == -1)
            {
                Console.WriteLine("");
            }
        }

    }

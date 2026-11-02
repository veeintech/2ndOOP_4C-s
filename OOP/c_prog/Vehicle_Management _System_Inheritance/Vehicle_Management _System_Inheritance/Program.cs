using System;

namespace Inheritance
{
    class vehicle
    {
        public string brand;
        public int year;

        public virtual void display()
        {
            Console.WriteLine($"Brand: {brand}");
            Console.WriteLine($"Year: {year}");
            return;
        }
    }

    class car : vehicle
    {
        public int numOfDoors;
        public override void display()
        {
            Console.WriteLine($"Brand: {brand}");
            Console.WriteLine($"Year: {year}");
            Console.WriteLine($"Number of Doors: {numOfDoors}");
        }
    }

    class motorcycle : vehicle
    {
        public string hasSideCar;
        public override void display()
        {
            Console.WriteLine($"Brand: {brand}");
            Console.WriteLine($"Year: {year}");
            Console.WriteLine($"Has Sidecar?: {hasSideCar}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Car Information: ");
            car c = new car();
            c.brand = "Ferrari";
            c.year = 2026;
            c.numOfDoors = 2;
            c.display();

            Console.WriteLine("\nMotorcycle Information: ");
            motorcycle m = new motorcycle();
            m.brand = "Honda";
            m.year = 2025;
            m.hasSideCar = "no";
            m.display();
        }
    }
}

using System;

internal class PayrollSystem
{
    static double ComGrossPay(double hoursW, double ratePerH)
    {
        return hoursW * ratePerH;
    }

    static double ComDeduc(double grossPay)
    {
        if (grossPay >= 20000)
        {
            return grossPay * 0.12;
        }
        else if (grossPay >= 10000)
        {
            return grossPay * 0.08;
        }
        else
        {
            return grossPay * 0.05;
        }
    }

    static double ComNetPay(double grossPay, double deduc)
    {
        return grossPay - deduc;
    }

    static void DisplayPayroll(double gross, double deduc, double net)
    {
        Console.WriteLine("\n\n==============================");
        Console.WriteLine($"GROSS PAY: PHP{gross:F2}");
        Console.WriteLine($"DEDUCTION: PHP{deduc:F2}");
        Console.WriteLine($"NET PAY: PHP{net:F2}");
        Console.WriteLine("==============================");
    }

    static void Main()
    {
        Console.Write("Enter Hours Worked: ");
        double hW = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter Rate per Hour: ");
        double ratePerH = Convert.ToDouble(Console.ReadLine());

        double gPay = ComGrossPay(hW, ratePerH);
        double deduc = ComDeduc(gPay);
        double netPay = ComNetPay(gPay, deduc);

        DisplayPayroll(gPay, deduc, netPay);
    }
}
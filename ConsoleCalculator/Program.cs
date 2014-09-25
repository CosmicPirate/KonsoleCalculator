using System;
using System.Collections.Generic;

namespace ConsoleCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            string invitation = "> ";
            Calculator calc = new Calculator();

            Console.Out.WriteLine("Для выхода введите exit или пустую строку.");
            Console.Out.Write(invitation);
            
            string input;
            while ((input = Console.In.ReadLine()).Length > 0 && input != "exit")
            {
                try
                {
                    Console.Out.WriteLine("= " + calc.Calculate(input));
                }
                catch (ArgumentException ex)
                {
                    Console.Out.WriteLine(ex.Message);
                }

                Console.Out.Write(invitation);
            }
        }

    }

}

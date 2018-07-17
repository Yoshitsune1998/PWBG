using System;
using System.Linq;

namespace PWBG_BOT
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            if (args.Any() && args.First() == "-version")
            {
                Console.WriteLine("Version 0.0.1");
                return;
            }
            Console.WriteLine("Hello World!");
        }
    }
}

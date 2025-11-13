using System;

namespace CarRental.ConsoleUI.Utils
{
    public static class ConsoleHelper
    {
        public static void WriteHeader(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('=', text.Length));
            Console.WriteLine(text);
            Console.WriteLine(new string('=', text.Length));
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void WriteInfo(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void WriteWarning(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void WriteError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void WaitForKeyPress(string message = "Press any key to continue...")
        {
            Console.WriteLine();
            Console.Write(message);
            Console.ReadKey(true);
        }
    }
}

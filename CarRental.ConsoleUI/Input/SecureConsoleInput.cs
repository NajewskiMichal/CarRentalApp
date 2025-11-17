using System;
using System.Text;

namespace CarRental.ConsoleUI.Input
{
    public static class SecureConsoleInput
    {
        
        public static string ReadPassword(string prompt)
        {
            Console.Write(prompt);
            var sb = new StringBuilder();

            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                    continue;
                }

                if (char.IsControl(key.KeyChar))
                    continue;

                sb.Append(key.KeyChar);
                Console.Write("*");
            }

            return sb.ToString();
        }
    }
}

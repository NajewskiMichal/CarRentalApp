using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CarRentalConsoleApp
{
    // Thrown when user wants to go back to the previous menu
    public class BackNavigationException : Exception
    {
        public BackNavigationException() : base("BACK") {}
    }

    public static class InputValidator
    {
        public static string ValidateInputNotEmpty(string prompt)
        {
            Console.Write(prompt);
            string input = Console.ReadLine().Trim();
            if (input.Equals("B", StringComparison.OrdinalIgnoreCase)) throw new BackNavigationException();
            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Field cannot be empty. Please try again (or press B to go back): ");
                input = Console.ReadLine().Trim();
            }
            return input;
        }

        public static int ValidateIntegerInput(string prompt)
        {
            Console.Write(prompt);
            int number;
            while (!int.TryParse(Console.ReadLine(), out number) || number <= 0)
            {
                Console.WriteLine("Invalid value. Please enter a positive integer (or press B to go back): ");
            }
            return number;
        }

        public static string ValidateEmail()
        {
            Console.Write("Email: ");
            string email = Console.ReadLine().Trim();
            while (!IsValidEmail(email))
            {
                Console.WriteLine("Invalid email format. Please enter a valid email (or press B to go back): ");
                email = Console.ReadLine().Trim();
            }
            return email;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string ValidateDateInput()
        {
            Console.Write("Date (yyyy-mm-dd): ");
            string input = Console.ReadLine().Trim();
            if (input.Equals("B", StringComparison.OrdinalIgnoreCase)) throw new BackNavigationException();
            while (!DateTime.TryParseExact(input, "yyyy-MM-dd", null, DateTimeStyles.None, out _))
            {
                Console.WriteLine("Invalid date format. Please use yyyy-mm-dd (or press B to go back): ");
                input = Console.ReadLine().Trim();
            }
            return input;
        }
    }
}
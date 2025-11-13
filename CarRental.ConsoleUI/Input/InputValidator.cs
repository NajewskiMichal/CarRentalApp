using System;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Input
{
    public class InputValidator : IInputValidator
    {
        public string ReadInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        public string ValidateInputNotEmpty(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();

                ConsoleHelper.WriteWarning("Value cannot be empty. Please try again.");
            }
        }

        public int ValidateIntegerInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();

                if (string.Equals(input, "B", StringComparison.OrdinalIgnoreCase))
                    throw new BackNavigationException();

                if (int.TryParse(input, out var value))
                    return value;

                ConsoleHelper.WriteWarning("Please enter a valid integer.");
            }
        }

        public int ValidateIntegerInput(string prompt, int minValue)
        {
            while (true)
            {
                var value = ValidateIntegerInput(prompt);
                if (value >= minValue)
                    return value;

                ConsoleHelper.WriteWarning($"Value must be at least {minValue}.");
            }
        }
    }
}

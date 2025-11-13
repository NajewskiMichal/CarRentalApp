using System;
using System.Collections.Generic;
using System.Linq;
using CarRental.ConsoleUI.Commands;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Menu
{
    public class MainMenu
    {
        private readonly IInputValidator _inputValidator;
        private readonly IReadOnlyList<IMenuCommand> _commands;
        private readonly IReadOnlyList<MenuOption> _options;

        public MainMenu(IInputValidator inputValidator, IReadOnlyList<IMenuCommand> commands)
        {
            _inputValidator = inputValidator;
            _commands = commands;

            _options = commands
                .Select(c => new MenuOption(c.Key, c.Description))
                .OrderBy(o => o.Key)
                .ToArray();
        }

        public void Display()
        {
            ConsoleHelper.WriteHeader("Main Menu");

            foreach (var option in _options)
            {
                Console.WriteLine($"{option.Key}. {option.Description}");
            }

            Console.WriteLine();
            Console.WriteLine("B. Back / Cancel (in submenus)");
        }
    }
}

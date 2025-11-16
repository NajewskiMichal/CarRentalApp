using System;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Authentication;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.User
{
    public class ViewUserDetailsCommand : IMenuCommand
    {
        public string Key => "16";
        public string Description => "View employee details (admin)";

        private readonly IInputValidator _input;
        private readonly IUserManagementService _userManagementService;

        public ViewUserDetailsCommand(IInputValidator input, IUserManagementService userManagementService)
        {
            _input = input;
            _userManagementService = userManagementService;
        }

        public async Task ExecuteAsync()
        {
            if (!UserContext.IsAdmin)
            {
                ConsoleHelper.WriteError("Only admin users can access employee management.");
                ConsoleHelper.WaitForKeyPress();
                return;
            }

            Console.Clear();
            ConsoleHelper.WriteHeader("Employee details");

            var id = _input.ValidateIntegerInput("User ID: ");

            var user = await _userManagementService.GetByIdAsync(id);
            if (user is null)
            {
                ConsoleHelper.WriteWarning($"User with ID={id} not found.");
            }
            else
            {
                Console.WriteLine($"ID      : {user.Id}");
                Console.WriteLine($"Username: {user.Username}");
                Console.WriteLine($"Email   : {user.Email}");
                Console.WriteLine($"Role    : {user.Role}");
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

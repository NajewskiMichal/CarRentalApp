using System;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Authentication;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;
using CarRental.Domain.Enums;

namespace CarRental.ConsoleUI.Commands.User
{
    public class ChangeUserRoleCommand : IMenuCommand
    {
        public string Key => "20";
        public string Description => "Change employee role / promote to admin (admin)";

        private readonly IInputValidator _input;
        private readonly IUserManagementService _userManagementService;

        public ChangeUserRoleCommand(IInputValidator input, IUserManagementService userManagementService)
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
            ConsoleHelper.WriteHeader("Change employee role");

            var id = _input.ValidateIntegerInput("User ID: ");

            Console.WriteLine("New role:");
            Console.WriteLine("1. Admin");
            Console.WriteLine("2. Employee");
            var roleChoice = _input.ReadInput("Choose role: ");

            var newRole = roleChoice == "1" ? UserRole.Admin : UserRole.Employee;

            try
            {
                await _userManagementService.ChangeRoleAsync(id, newRole);
                ConsoleHelper.WriteInfo($"User role changed to {newRole}.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to change user role.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

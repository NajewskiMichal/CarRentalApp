using System;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Authentication;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;
using CarRental.Domain.Enums;

namespace CarRental.ConsoleUI.Commands.User
{
    public class AddUserCommand : IMenuCommand
    {
        public string Key => "17";
        public string Description => "Add employee/user (admin)";

        private readonly IInputValidator _input;
        private readonly IUserManagementService _userManagementService;

        public AddUserCommand(IInputValidator input, IUserManagementService userManagementService)
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
            ConsoleHelper.WriteHeader("Add employee / user");

            var username = _input.ValidateInputNotEmpty("Username: ");
            var email = _input.ValidateInputNotEmpty("Email: ");
            var password = _input.ValidateInputNotEmpty("Password: ");

            Console.WriteLine("Role:");
            Console.WriteLine("1. Admin");
            Console.WriteLine("2. Employee");
            var roleChoice = _input.ReadInput("Choose role: ");
            var role = roleChoice == "1" ? UserRole.Admin : UserRole.Employee;

            try
            {
                var user = await _userManagementService.CreateAsync(username, email, password, role);
                ConsoleHelper.WriteInfo($"User created. ID={user.Id}, {user.Username} ({user.Role})");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to create user.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

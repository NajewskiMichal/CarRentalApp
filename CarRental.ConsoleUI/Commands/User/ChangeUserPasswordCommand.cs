using System;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Authentication;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.User
{
    public class ChangeUserPasswordCommand : IMenuCommand
    {
        public string Key => "19";
        public string Description => "Change employee password (admin)";

        private readonly IInputValidator _input;
        private readonly IUserManagementService _userManagementService;

        public ChangeUserPasswordCommand(IInputValidator input, IUserManagementService userManagementService)
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
            ConsoleHelper.WriteHeader("Change employee password");

            var id = _input.ValidateIntegerInput("User ID: ");

            var newPassword = SecureConsoleInput.ReadPassword("New password: ");
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ConsoleHelper.WriteError("Password cannot be empty.");
                ConsoleHelper.WaitForKeyPress();
                return;
            }

            try
            {
                await _userManagementService.ChangePasswordAsync(id, newPassword);
                ConsoleHelper.WriteInfo("Password changed successfully.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to change password.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

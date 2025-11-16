using System;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Authentication;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.User
{
    public class EditUserCommand : IMenuCommand
    {
        public string Key => "18";
        public string Description => "Edit employee username/email (admin)";

        private readonly IInputValidator _input;
        private readonly IUserManagementService _userManagementService;

        public EditUserCommand(IInputValidator input, IUserManagementService userManagementService)
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
            ConsoleHelper.WriteHeader("Edit employee");

            var id = _input.ValidateIntegerInput("User ID: ");

            var newUsername = _input.ReadInput("New username (leave empty to keep current): ");
            var newEmail = _input.ReadInput("New email (leave empty to keep current): ");

            string? usernameToSet = string.IsNullOrWhiteSpace(newUsername) ? null : newUsername;
            string? emailToSet = string.IsNullOrWhiteSpace(newEmail) ? null : newEmail;

            try
            {
                var user = await _userManagementService.UpdateAsync(id, usernameToSet, emailToSet);
                ConsoleHelper.WriteInfo(
                    $"User updated. ID={user.Id}, {user.Username} ({user.Email}, {user.Role})");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to update user.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

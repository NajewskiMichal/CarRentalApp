using System;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Authentication;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.User
{
    public class DeleteUserCommand : IMenuCommand
    {
        public string Key => "21";
        public string Description => "Delete employee/user (admin)";

        private readonly IInputValidator _input;
        private readonly IUserManagementService _userManagementService;

        public DeleteUserCommand(IInputValidator input, IUserManagementService userManagementService)
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
            ConsoleHelper.WriteHeader("Delete employee / user");

            var id = _input.ValidateIntegerInput("User ID: ");

            var confirmation = _input.ReadInput("Are you sure you want to delete this user? (Y/N): ");
            if (!string.Equals(confirmation, "Y", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleHelper.WriteInfo("Operation cancelled.");
                ConsoleHelper.WaitForKeyPress();
                return;
            }

            try
            {
                await _userManagementService.DeleteAsync(id);
                ConsoleHelper.WriteInfo("User deleted.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to delete user.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

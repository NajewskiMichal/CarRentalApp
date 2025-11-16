using System;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Authentication;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.User
{
    public class ListUsersCommand : IMenuCommand
    {
        public string Key => "15";
        public string Description => "List employees/users (admin)";

        private readonly IUserManagementService _userManagementService;

        public ListUsersCommand(IUserManagementService userManagementService)
        {
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
            ConsoleHelper.WriteHeader("Employees / Users");

            var users = await _userManagementService.GetAllAsync();
            if (users.Count == 0)
            {
                ConsoleHelper.WriteWarning("No users found.");
            }
            else
            {
                foreach (var user in users)
                {
                    Console.WriteLine($"ID={user.Id} | {user.Username} | {user.Email} | Role={user.Role}");
                }
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

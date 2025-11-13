using System;
using System.Threading.Tasks;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;
using CarRental.Domain.Enums;

namespace CarRental.ConsoleUI.Authentication
{
    /// <summary>
    /// Simple login/register screen.
    /// </summary>
    public class LoginScreen
    {
        private readonly IAuthService _authService;
        private readonly IInputValidator _inputValidator;

        public LoginScreen(IAuthService authService, IInputValidator inputValidator)
        {
            _authService = authService;
            _inputValidator = inputValidator;
        }

        public async Task<UserDto?> ShowAsync()
        {
            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteHeader("Authentication");

                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register new user");
                Console.WriteLine("0. Exit");
                Console.WriteLine();

                var choice = _inputValidator.ReadInput("Choose an option: ");

                switch (choice)
                {
                    case "1":
                        {
                            var user = await LoginAsync();
                            if (user != null)
                                return user;
                            break;
                        }
                    case "2":
                        await RegisterAsync();
                        break;
                    case "0":
                        return null;
                    default:
                        ConsoleHelper.WriteWarning("Invalid option. Please try again.");
                        ConsoleHelper.WaitForKeyPress();
                        break;
                }
            }
        }

        private async Task<UserDto?> LoginAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Login");

            var username = _inputValidator.ValidateInputNotEmpty("Username: ");
            Console.Write("Password: ");
            var password = ReadPassword();

            var user = await _authService.LoginAsync(username, password);
            if (user is null)
            {
                ConsoleHelper.WriteError("Invalid username or password.");
                ConsoleHelper.WaitForKeyPress();
                return null;
            }

            return user;
        }

        private async Task RegisterAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Register");

            var username = _inputValidator.ValidateInputNotEmpty("Username: ");
            var email = _inputValidator.ValidateInputNotEmpty("Email: ");

            Console.Write("Password: ");
            var password = ReadPassword();
            Console.WriteLine();
            Console.Write("Confirm password: ");
            var confirmPassword = ReadPassword();

            if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ConsoleHelper.WriteError("Passwords do not match.");
                ConsoleHelper.WaitForKeyPress();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Select role:");
            Console.WriteLine("1. Admin");
            Console.WriteLine("2. Employee");
            var roleChoice = _inputValidator.ReadInput("Role: ");

            var role = roleChoice == "1" ? UserRole.Admin : UserRole.Employee;

            try
            {
                var user = await _authService.RegisterAsync(username, email, password, role);
                ConsoleHelper.WriteInfo($"User registered successfully: {user.Username} ({user.Role})");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Registration failed.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }

        private static string ReadPassword()
        {
            var password = string.Empty;
            ConsoleKeyInfo keyInfo;

            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password[..^1];
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    password += keyInfo.KeyChar;
                    Console.Write("*");
                }
            }

            Console.WriteLine();
            return password;
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using CarRental.ConsoleUI.Authentication;
using CarRental.ConsoleUI.CompositionRoot;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Menu;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var bootstrapper = new Bootstrapper();
            var app = bootstrapper.Build();

            // Initialize database
            await app.DatabaseInitializer.InitializeAsync();

            var inputValidator = app.InputValidator;
            var mainMenu = new MainMenu(inputValidator, app.Commands);

            // Initial login
            var user = await app.LoginScreen.ShowAsync();
            if (user is null)
            {
                ConsoleHelper.WriteInfo("Exiting application...");
                return;
            }

            // Set current user context (used for admin checks)
            UserContext.SetCurrentUser(user);

            ConsoleHelper.WriteInfo($"Logged in as: {user.Username} ({user.Role})");
            ConsoleHelper.WaitForKeyPress();

            // Main loop
            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteHeader("Car Rental System");

                mainMenu.Display();

                var choice = inputValidator.ReadInput("Choose an option: ");
                var command = app.Commands.SingleOrDefault(
                    c => c.Key.Equals(choice, StringComparison.OrdinalIgnoreCase));

                if (command == null)
                {
                    ConsoleHelper.WriteWarning("Invalid option. Please try again.");
                    ConsoleHelper.WaitForKeyPress();
                    continue;
                }

                try
                {
                    await command.ExecuteAsync();
                }
                catch (BackNavigationException)
                {
                    // User pressed 'B' somewhere – just go back to main menu
                }
                catch (LogoutRequestedException)
                {
                    // Clear current user and start a new login flow
                    UserContext.SetCurrentUser(null);

                    var newUser = await app.LoginScreen.ShowAsync();
                    if (newUser is null)
                    {
                        ConsoleHelper.WriteInfo("Exiting application...");
                        return;
                    }

                    UserContext.SetCurrentUser(newUser);
                    ConsoleHelper.WriteInfo($"Logged in as: {newUser.Username} ({newUser.Role})");
                    ConsoleHelper.WaitForKeyPress();
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteError("Unexpected error occurred.");
                    Console.WriteLine(ex.Message);
                    ConsoleHelper.WaitForKeyPress();
                }
            }
        }
    }
}

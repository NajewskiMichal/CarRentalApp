using System.Threading.Tasks;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.System
{
    /// <summary>
    /// Command that logs out the current user and returns to the login screen.
    /// </summary>
    public class LogoutCommand : IMenuCommand
    {
        // Letter key so it is clearly distinct from numeric options.
        public string Key => "L";
        public string Description => "Logout (switch user)";

        public Task ExecuteAsync()
        {
            ConsoleHelper.WriteInfo("Logging out from current session...");
            throw new LogoutRequestedException();
        }
    }
}

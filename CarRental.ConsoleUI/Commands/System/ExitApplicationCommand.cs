using System.Threading.Tasks;

namespace CarRental.ConsoleUI.Commands.System
{
    public class ExitApplicationCommand : IMenuCommand
    {
        public string Key => "0";
        public string Description => "Exit application";

        public Task ExecuteAsync()
        {
            // exit the process
            Environment.Exit(0);
            return Task.CompletedTask;
        }
    }
}

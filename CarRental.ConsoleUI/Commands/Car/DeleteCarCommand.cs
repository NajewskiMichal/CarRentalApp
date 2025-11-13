using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Car
{
    public class DeleteCarCommand : IMenuCommand
    {
        public string Key => "4";
        public string Description => "Delete car";

        private readonly IInputValidator _input;
        private readonly ICarService _carService;

        public DeleteCarCommand(IInputValidator input, ICarService carService)
        {
            _input = input;
            _carService = carService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Delete Car");

            var id = _input.ValidateIntegerInput("Car ID: ", 1);

            Console.Write("Are you sure? (y/N): ");
            var confirm = Console.ReadLine();

            if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleHelper.WriteInfo("Deletion cancelled.");
                ConsoleHelper.WaitForKeyPress();
                return;
            }

            try
            {
                await _carService.DeleteAsync(id);
                ConsoleHelper.WriteInfo("Car deleted successfully.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to delete car.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Car
{
    public class ListCarsCommand : IMenuCommand
    {
        public string Key => "1";
        public string Description => "List all cars";

        private readonly ICarService _carService;

        public ListCarsCommand(ICarService carService)
        {
            _carService = carService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Cars");

            var cars = await _carService.GetAllAsync();

            if (cars.Count == 0)
            {
                ConsoleHelper.WriteWarning("No cars found.");
            }
            else
            {
                EntityListPrinter.PrintCars(cars);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

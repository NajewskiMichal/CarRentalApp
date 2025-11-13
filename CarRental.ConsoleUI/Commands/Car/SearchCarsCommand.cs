using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Car
{
    public class SearchCarsCommand : IMenuCommand
    {
        public string Key => "5";
        public string Description => "Search cars by brand";

        private readonly IInputValidator _input;
        private readonly ICarService _carService;

        public SearchCarsCommand(IInputValidator input, ICarService carService)
        {
            _input = input;
            _carService = carService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Search Cars");

            var brand = _input.ValidateInputNotEmpty("Brand (fragment): ");

            var cars = await _carService.SearchByBrandAsync(brand);

            if (cars.Count == 0)
            {
                ConsoleHelper.WriteWarning("No cars matching the criteria.");
            }
            else
            {
                foreach (var car in cars)
                {
                    Console.WriteLine(
                        $"ID={car.Id} | {car.Brand} {car.Model} ({car.Year}) | VIN={car.Vin}");
                }
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

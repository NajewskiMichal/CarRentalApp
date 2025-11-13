using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;


namespace CarRental.ConsoleUI.Commands.Car
{
    public class AddCarCommand : IMenuCommand
    {
        public string Key => "2";
        public string Description => "Add new car";

        private readonly IInputValidator _input;
        private readonly ICarService _carService;

        public AddCarCommand(IInputValidator input, ICarService carService)
        {
            _input = input;
            _carService = carService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Add Car");

            var brand = _input.ValidateInputNotEmpty("Brand: ");
            var model = _input.ValidateInputNotEmpty("Model: ");
            var year = _input.ValidateIntegerInput("Year: ", 1900);
            var vin = _input.ValidateInputNotEmpty("VIN: ");

            try
            {
                var car = await _carService.AddAsync(brand, model, year, vin);
                ConsoleHelper.WriteInfo($"Car added. ID={car.Id}, {car.Brand} {car.Model} ({car.Year})");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to add car.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

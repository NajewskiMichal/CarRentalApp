using System;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Car
{
    public class EditCarCommand : IMenuCommand
    {
        public string Key => "3";
        public string Description => "Edit existing car";

        private readonly IInputValidator _input;
        private readonly ICarService _carService;

        public EditCarCommand(IInputValidator input, ICarService carService)
        {
            _input = input;
            _carService = carService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Edit Car");

            var cars = await _carService.GetAllAsync();
            if (cars.Count == 0)
            {
                ConsoleHelper.WriteWarning("No cars found. There is nothing to edit.");
                ConsoleHelper.WaitForKeyPress();
                return;
            }

            ConsoleHelper.WriteInfo("Available cars:");
            EntityListPrinter.PrintCars(cars);
            Console.WriteLine();

            int id;
            while (true)
            {
                id = _input.ValidateIntegerInput("Car ID to edit: ", 1);

                if (cars.Any(c => c.Id == id))
                {
                    break;
                }

                ConsoleHelper.WriteWarning("Car with given ID does not exist. Please enter an ID from the list above.");
            }

            try
            {
                var existing = await _carService.GetByIdAsync(id);
                ConsoleHelper.WriteInfo(
                    $"Editing: {existing.Brand} {existing.Model} ({existing.Year}), VIN={existing.Vin}");

                var brand = _input.ValidateInputNotEmpty($"Brand [{existing.Brand}]: ");
                var model = _input.ValidateInputNotEmpty($"Model [{existing.Model}]: ");
                var year = _input.ValidateIntegerInput($"Year [{existing.Year}]: ", 1900);
                var vin = _input.ValidateInputNotEmpty($"VIN [{existing.Vin}]: ");

                await _carService.UpdateAsync(id, brand, model, year, vin);
                ConsoleHelper.WriteInfo("Car updated successfully.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to edit car.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

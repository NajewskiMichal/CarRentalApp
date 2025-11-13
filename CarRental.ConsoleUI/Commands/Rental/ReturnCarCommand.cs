using System;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Rental
{
    public class ReturnCarCommand : IMenuCommand
    {
        public string Key => "13";
        public string Description => "Return a car";

        private readonly IInputValidator _input;
        private readonly IRentalService _rentalService;

        public ReturnCarCommand(IInputValidator input, IRentalService rentalService)
        {
            _input = input;
            _rentalService = rentalService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Return Car");

            var rentalId = _input.ValidateIntegerInput("Rental ID: ", 1);
            var returnDate = DateTime.Now;

            try
            {
                await _rentalService.ReturnCarAsync(rentalId, returnDate);
                ConsoleHelper.WriteInfo("Car returned successfully.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to return car.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

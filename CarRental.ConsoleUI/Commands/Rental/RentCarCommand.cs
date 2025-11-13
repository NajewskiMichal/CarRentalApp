using System;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Rental
{
    public class RentCarCommand : IMenuCommand
    {
        public string Key => "12";
        public string Description => "Rent a car";

        private readonly IInputValidator _input;
        private readonly IRentalService _rentalService;

        public RentCarCommand(IInputValidator input, IRentalService rentalService)
        {
            _input = input;
            _rentalService = rentalService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Rent Car");

            var customerId = _input.ValidateIntegerInput("Customer ID: ", 1);
            var carId = _input.ValidateIntegerInput("Car ID: ", 1);

            var rentDate = DateTime.Now;

            try
            {
                var rental = await _rentalService.RentCarAsync(customerId, carId, rentDate);
                ConsoleHelper.WriteInfo($"Car rented. Rental ID={rental.Id}, Date={rental.RentDate}");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to rent car.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

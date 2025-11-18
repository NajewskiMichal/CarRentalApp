using System;
using System.Linq;
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

            var activeRentals = await _rentalService.GetActiveAsync();
            if (activeRentals.Count == 0)
            {
                ConsoleHelper.WriteWarning("There are no active rentals.");
                ConsoleHelper.WaitForKeyPress();
                return;
            }

            ConsoleHelper.WriteInfo("Active rentals:");
            EntityListPrinter.PrintRentals(activeRentals);
            Console.WriteLine();

            int rentalId;
            while (true)
            {
                rentalId = _input.ValidateIntegerInput("Rental ID to close: ", 1);

                if (activeRentals.Any(r => r.Id == rentalId))
                {
                    break;
                }

                ConsoleHelper.WriteWarning("Rental with given ID does not exist. Please enter an ID from the list above.");
            }

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

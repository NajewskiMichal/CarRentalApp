using System;
using System.Linq;
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
        private readonly ICustomerService _customerService;
        private readonly ICarService _carService;

        public RentCarCommand(
            IInputValidator input,
            IRentalService rentalService,
            ICustomerService customerService,
            ICarService carService)
        {
            _input = input;
            _rentalService = rentalService;
            _customerService = customerService;
            _carService = carService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Rent Car");

            var customers = await _customerService.GetAllAsync();
            if (customers.Count == 0)
            {
                ConsoleHelper.WriteWarning("No customers found. You need to add a customer first.");
                ConsoleHelper.WaitForKeyPress();
                return;
            }

            var cars = await _carService.GetAllAsync();
            if (cars.Count == 0)
            {
                ConsoleHelper.WriteWarning("No cars found. You need to add a car first.");
                ConsoleHelper.WaitForKeyPress();
                return;
            }

            ConsoleHelper.WriteInfo("Available customers:");
            EntityListPrinter.PrintCustomers(customers);
            Console.WriteLine();

            int customerId;
            while (true)
            {
                customerId = _input.ValidateIntegerInput("Customer ID: ", 1);

                if (customers.Any(c => c.Id == customerId))
                {
                    break;
                }

                ConsoleHelper.WriteWarning("Customer with given ID does not exist. Please enter an ID from the list above.");
            }

            Console.WriteLine();
            ConsoleHelper.WriteInfo("Available cars:");
            EntityListPrinter.PrintCars(cars);
            Console.WriteLine();

            int carId;
            while (true)
            {
                carId = _input.ValidateIntegerInput("Car ID: ", 1);

                if (cars.Any(c => c.Id == carId))
                {
                    break;
                }

                ConsoleHelper.WriteWarning("Car with given ID does not exist. Please enter an ID from the list above.");
            }

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

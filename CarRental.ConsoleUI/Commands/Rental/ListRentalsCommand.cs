using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Rental
{
    public class ListRentalsCommand : IMenuCommand
    {
        public string Key => "11";
        public string Description => "List rentals";

        private readonly IRentalService _rentalService;

        public ListRentalsCommand(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Rentals");

            var rentals = await _rentalService.GetAllAsync();

            if (rentals.Count == 0)
            {
                ConsoleHelper.WriteWarning("No rentals found.");
            }
            else
            {
                foreach (var rental in rentals)
                {
                    var status = rental.IsActive ? "ACTIVE" : "CLOSED";
                    Console.WriteLine(
                        $"ID={rental.Id} | CustomerId={rental.CustomerId} | CarId={rental.CarId} | Rent={rental.RentDate} | Return={rental.ReturnDate} | {status}");
                }
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

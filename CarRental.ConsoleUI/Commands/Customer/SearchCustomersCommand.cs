using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Customer
{
    public class SearchCustomersCommand : IMenuCommand
    {
        public string Key => "10";
        public string Description => "Search customers by name";

        private readonly IInputValidator _input;
        private readonly ICustomerService _customerService;

        public SearchCustomersCommand(IInputValidator input, ICustomerService customerService)
        {
            _input = input;
            _customerService = customerService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Search Customers");

            var name = _input.ValidateInputNotEmpty("Name (fragment): ");

            var customers = await _customerService.SearchByNameAsync(name);

            if (customers.Count == 0)
            {
                ConsoleHelper.WriteWarning("No customers matching the criteria.");
            }
            else
            {
                foreach (var customer in customers)
                {
                    Console.WriteLine($"ID={customer.Id} | {customer.Name} | {customer.Email}");
                }
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Customer
{
    public class ListCustomersCommand : IMenuCommand
    {
        public string Key => "6";
        public string Description => "List all customers";

        private readonly ICustomerService _customerService;

        public ListCustomersCommand(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Customers");

            var customers = await _customerService.GetAllAsync();

            if (customers.Count == 0)
            {
                ConsoleHelper.WriteWarning("No customers found.");
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

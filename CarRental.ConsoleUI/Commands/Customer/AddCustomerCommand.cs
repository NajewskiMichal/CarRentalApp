using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Customer
{
    public class AddCustomerCommand : IMenuCommand
    {
        public string Key => "7";
        public string Description => "Add new customer";

        private readonly IInputValidator _input;
        private readonly ICustomerService _customerService;

        public AddCustomerCommand(IInputValidator input, ICustomerService customerService)
        {
            _input = input;
            _customerService = customerService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Add Customer");

            var name = _input.ValidateInputNotEmpty("Name: ");
            var email = _input.ValidateInputNotEmpty("Email: ");

            try
            {
                var customer = await _customerService.AddAsync(name, email);
                ConsoleHelper.WriteInfo($"Customer added. ID={customer.Id}, {customer.Name} ({customer.Email})");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to add customer.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

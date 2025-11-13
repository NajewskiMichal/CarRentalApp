using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Customer
{
    public class DeleteCustomerCommand : IMenuCommand
    {
        public string Key => "9";
        public string Description => "Delete customer";

        private readonly IInputValidator _input;
        private readonly ICustomerService _customerService;

        public DeleteCustomerCommand(IInputValidator input, ICustomerService customerService)
        {
            _input = input;
            _customerService = customerService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Delete Customer");

            var id = _input.ValidateIntegerInput("Customer ID: ", 1);

            Console.Write("Are you sure? (y/N): ");
            var confirm = Console.ReadLine();

            if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleHelper.WriteInfo("Deletion cancelled.");
                ConsoleHelper.WaitForKeyPress();
                return;
            }

            try
            {
                await _customerService.DeleteAsync(id);
                ConsoleHelper.WriteInfo("Customer deleted successfully.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to delete customer.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

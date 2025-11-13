using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Input;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Customer
{
    public class EditCustomerCommand : IMenuCommand
    {
        public string Key => "8";
        public string Description => "Edit existing customer";

        private readonly IInputValidator _input;
        private readonly ICustomerService _customerService;

        public EditCustomerCommand(IInputValidator input, ICustomerService customerService)
        {
            _input = input;
            _customerService = customerService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Edit Customer");

            var id = _input.ValidateIntegerInput("Customer ID: ", 1);

            try
            {
                var existing = await _customerService.GetByIdAsync(id);
                ConsoleHelper.WriteInfo($"Editing: {existing.Name} ({existing.Email})");

                var name = _input.ValidateInputNotEmpty($"Name [{existing.Name}]: ");
                var email = _input.ValidateInputNotEmpty($"Email [{existing.Email}]: ");

                await _customerService.UpdateAsync(id, name, email);
                ConsoleHelper.WriteInfo("Customer updated successfully.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Failed to edit customer.");
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

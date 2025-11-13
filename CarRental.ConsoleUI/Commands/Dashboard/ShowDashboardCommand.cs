using System.Threading.Tasks;
using CarRental.Application.Interfaces.Services;
using CarRental.ConsoleUI.Utils;

namespace CarRental.ConsoleUI.Commands.Dashboard
{
    public class ShowDashboardCommand : IMenuCommand
    {
        public string Key => "14";
        public string Description => "Show dashboard summary";

        private readonly IDashboardService _dashboardService;

        public ShowDashboardCommand(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task ExecuteAsync()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("Dashboard");

            var summary = await _dashboardService.GetSummaryAsync();

            Console.WriteLine($"Total cars:       {summary.TotalCars}");
            Console.WriteLine($"Total customers:  {summary.TotalCustomers}");
            Console.WriteLine($"Active rentals:   {summary.ActiveRentals}");

            ConsoleHelper.WaitForKeyPress();
        }
    }
}

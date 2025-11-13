namespace CarRental.Application.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalCars { get; init; }
        public int TotalCustomers { get; init; }
        public int ActiveRentals { get; init; }

        public DashboardSummaryDto(int totalCars, int totalCustomers, int activeRentals)
        {
            TotalCars = totalCars;
            TotalCustomers = totalCustomers;
            ActiveRentals = activeRentals;
        }
    }
}

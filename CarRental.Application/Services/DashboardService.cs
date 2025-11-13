using System.Linq;
using System.Threading.Tasks;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Interfaces.Services;

namespace CarRental.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ICarRepository _carRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRentalRepository _rentalRepository;

        public DashboardService(
            ICarRepository carRepository,
            ICustomerRepository customerRepository,
            IRentalRepository rentalRepository)
        {
            _carRepository = carRepository;
            _customerRepository = customerRepository;
            _rentalRepository = rentalRepository;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var carsTask = _carRepository.GetAllAsync();
            var customersTask = _customerRepository.GetAllAsync();
            var rentalsTask = _rentalRepository.GetAllAsync();

            await Task.WhenAll(carsTask, customersTask, rentalsTask);

            var cars = carsTask.Result;
            var customers = customersTask.Result;
            var rentals = rentalsTask.Result;

            var activeRentals = rentals.Count(r => r.IsActive);

            return new DashboardSummaryDto(
                totalCars: cars.Count,
                totalCustomers: customers.Count,
                activeRentals: activeRentals);
        }
    }
}

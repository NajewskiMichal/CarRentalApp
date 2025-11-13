using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Application.DTOs;
using CarRental.Application.Exceptions;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Interfaces.Services;
using CarRental.Domain.Entities;

namespace CarRental.Application.Services
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly ICarRepository _carRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger _logger;

        public RentalService(
            IRentalRepository rentalRepository,
            ICarRepository carRepository,
            ICustomerRepository customerRepository,
            ILogger logger)
        {
            _rentalRepository = rentalRepository;
            _carRepository = carRepository;
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<RentalDto> RentCarAsync(int customerId, int carId, DateTime rentDate)
        {
            var errors = new List<string>();

            if (customerId <= 0)
                errors.Add("Customer ID must be greater than zero.");
            if (carId <= 0)
                errors.Add("Car ID must be greater than zero.");
            if (rentDate == default)
                errors.Add("Rent date is required.");

            if (errors.Count > 0)
                throw new ValidationException(errors);

            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer is null)
                throw new NotFoundException(nameof(Customer), customerId);

            var car = await _carRepository.GetByIdAsync(carId);
            if (car is null)
                throw new NotFoundException(nameof(Car), carId);

            var activeRentals = await _rentalRepository.GetActiveRentalsAsync();
            if (activeRentals.Any(r => r.CarId == carId))
                throw new ValidationException(new[] { "Car is already rented." });

            var rental = new Rental(customerId, carId, rentDate);
            await _rentalRepository.AddAsync(rental);

            _logger.Info($"Car rented: CarId={carId}, CustomerId={customerId}, Date={rentDate:O}");

            return MapToDto(rental);
        }

        public async Task ReturnCarAsync(int rentalId, DateTime returnDate)
        {
            if (rentalId <= 0)
                throw new ValidationException(new[] { "Rental ID must be greater than zero." });

            if (returnDate == default)
                throw new ValidationException(new[] { "Return date is required." });

            var rental = await _rentalRepository.GetByIdAsync(rentalId);
            if (rental is null)
                throw new NotFoundException(nameof(Rental), rentalId);

            rental.Return(returnDate);
            await _rentalRepository.UpdateAsync(rental);

            _logger.Info($"Car returned: RentalId={rentalId}, ReturnDate={returnDate:O}");
        }

        public async Task<IReadOnlyList<RentalDto>> GetAllAsync()
        {
            var rentals = await _rentalRepository.GetAllAsync();
            return rentals.Select(MapToDto).ToArray();
        }

        public async Task<IReadOnlyList<RentalDto>> GetActiveAsync()
        {
            var rentals = await _rentalRepository.GetActiveRentalsAsync();
            return rentals.Select(MapToDto).ToArray();
        }

        private static RentalDto MapToDto(Rental rental) =>
            new RentalDto
            {
                Id = rental.Id,
                CustomerId = rental.CustomerId,
                CarId = rental.CarId,
                RentDate = rental.RentDate,
                ReturnDate = rental.ReturnDate,
                IsActive = rental.IsActive
            };
    }
}

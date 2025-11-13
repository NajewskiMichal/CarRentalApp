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
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly ILogger _logger;

        public CarService(ICarRepository carRepository, ILogger logger)
        {
            _carRepository = carRepository;
            _logger = logger;
        }

        public async Task<IReadOnlyList<CarDto>> GetAllAsync()
        {
            var cars = await _carRepository.GetAllAsync();
            return cars.Select(MapToDto).ToArray();
        }

        public async Task<CarDto> GetByIdAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car is null)
                throw new NotFoundException(nameof(Car), id);

            return MapToDto(car);
        }

        public async Task<CarDto> AddAsync(string brand, string model, int year, string vin)
        {
            ValidateCarInput(brand, model, year, vin);

            var car = new Car(brand, model, year, vin);
            await _carRepository.AddAsync(car);

            _logger.Info($"Car added: {car.Brand} {car.Model} ({car.Year}), VIN={car.Vin}");

            return MapToDto(car);
        }

        public async Task UpdateAsync(int id, string brand, string model, int year, string vin)
        {
            ValidateCarInput(brand, model, year, vin);

            var car = await _carRepository.GetByIdAsync(id);
            if (car is null)
                throw new NotFoundException(nameof(Car), id);

            // Tworzymy nową instancję – można też dodać metody update w encji.
            var updatedCar = new Car(id, brand, model, year, vin);
            await _carRepository.UpdateAsync(updatedCar);

            _logger.Info($"Car updated: ID={id}");
        }

        public async Task DeleteAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car is null)
                throw new NotFoundException(nameof(Car), id);

            await _carRepository.DeleteAsync(id);
            _logger.Info($"Car deleted: ID={id}");
        }

        public async Task<IReadOnlyList<CarDto>> SearchByBrandAsync(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
                return new List<CarDto>();

            var cars = await _carRepository.SearchByBrandAsync(brand.Trim());
            return cars.Select(MapToDto).ToArray();
        }

        private static void ValidateCarInput(string brand, string model, int year, string vin)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(brand))
                errors.Add("Brand is required.");

            if (string.IsNullOrWhiteSpace(model))
                errors.Add("Model is required.");

            if (year < 1950 || year > 2100)
                errors.Add("Year is out of valid range.");

            if (string.IsNullOrWhiteSpace(vin))
                errors.Add("VIN is required.");

            if (errors.Count > 0)
                throw new ValidationException(errors);
        }

        private static CarDto MapToDto(Car car) =>
            new CarDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Vin = car.Vin
            };
    }
}

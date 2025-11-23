using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Services;
using CarRental.Domain.Entities;
using CarRental.Domain.ValueObjects;
using Moq;
using Xunit;

namespace CarRental.Tests.Application
{
    public class DashboardServiceTests
    {
        [Fact]
        public async Task GetSummaryAsync_ReturnsAggregatedCounts()
        {
            var carRepository = new Mock<ICarRepository>();
            var customerRepository = new Mock<ICustomerRepository>();
            var rentalRepository = new Mock<IRentalRepository>();

            carRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Car>
                {
                    new Car(1, "Toyota", "Corolla", 2020, "VIN1"),
                    new Car(2, "Honda", "Civic", 2021, "VIN2")
                });

            customerRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Customer>
                {
                    new Customer(1, "Alice", Email.Create("alice@example.com")),
                    new Customer(2, "Bob", Email.Create("bob@example.com")),
                    new Customer(3, "Charlie", Email.Create("charlie@example.com"))
                });

            rentalRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Rental>
                {
                    new Rental(1, 1, 1, DateTime.Today),
                    new Rental(2, 2, 2, DateTime.Today)
                });

            var sut = new DashboardService(
                carRepository.Object,
                customerRepository.Object,
                rentalRepository.Object);

            var summary = await sut.GetSummaryAsync();

            Assert.Equal(2, summary.TotalCars);
            Assert.Equal(3, summary.TotalCustomers);
            Assert.Equal(2, summary.ActiveRentals);
        }
    }
}

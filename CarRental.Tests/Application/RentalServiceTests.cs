using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;
using CarRental.Application.Exceptions;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Services;
using CarRental.Domain.Entities;
using CarRental.Domain.ValueObjects;
using Moq;
using Xunit;

namespace CarRental.Tests.Application
{
    public class RentalServiceTests
    {
        private readonly Mock<IRentalRepository> _rentalRepositoryMock;
        private readonly Mock<ICarRepository> _carRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly RentalService _sut;

        public RentalServiceTests()
        {
            _rentalRepositoryMock = new Mock<IRentalRepository>();
            _carRepositoryMock = new Mock<ICarRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger>();

            _sut = new RentalService(
                _rentalRepositoryMock.Object,
                _carRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task RentCarAsync_WithInvalidParameters_ThrowsValidationException()
        {
            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.RentCarAsync(0, 0, default));
        }

        [Fact]
        public async Task RentCarAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
        {
            _customerRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Customer?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.RentCarAsync(1, 1, DateTime.Today));
        }

        [Fact]
        public async Task RentCarAsync_WhenCarDoesNotExist_ThrowsNotFoundException()
        {
            _customerRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Customer(1, "Alice", Email.Create("a@a.com")));

            _carRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Car?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.RentCarAsync(1, 1, DateTime.Today));
        }

        [Fact]
        public async Task RentCarAsync_WhenCarAlreadyRented_ThrowsValidationException()
        {
            _customerRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Customer(1, "Alice", Email.Create("a@a.com")));

            _carRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Car(1, "Toyota", "Corolla", 2020, "VIN"));

            _rentalRepositoryMock
                .Setup(r => r.GetActiveRentalsAsync())
                .ReturnsAsync(new List<Rental>
                {
                    new Rental(1, 1, DateTime.Today)
                });

            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.RentCarAsync(1, 1, DateTime.Today));
        }

        [Fact]
        public async Task RentCarAsync_WithValidData_CreatesRentalAndLogs()
        {
            _customerRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Customer(1, "Alice", Email.Create("a@a.com")));

            _carRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Car(1, "Toyota", "Corolla", 2020, "VIN"));

            _rentalRepositoryMock
                .Setup(r => r.GetActiveRentalsAsync())
                .ReturnsAsync(new List<Rental>());

            Rental? addedRental = null;

            _rentalRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Rental>()))
                .Callback<Rental>(r => addedRental = r)
                .Returns(Task.CompletedTask);

            var rentDate = new DateTime(2024, 1, 10);

            var dto = await _sut.RentCarAsync(1, 1, rentDate);

            Assert.NotNull(addedRental);
            Assert.Equal(1, addedRental!.CustomerId);
            Assert.Equal(1, addedRental.CarId);
            Assert.Equal(rentDate, addedRental.RentDate);

            Assert.Equal(addedRental.Id, dto.Id);
            Assert.Equal(addedRental.CustomerId, dto.CustomerId);
            Assert.Equal(addedRental.CarId, dto.CarId);
            Assert.Equal(addedRental.RentDate, dto.RentDate);

            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("Car rented"))),
                Times.Once);
        }

        [Fact]
        public async Task ReturnCarAsync_WithInvalidParameters_ThrowsValidationException()
        {
            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.ReturnCarAsync(0, default));
        }

        [Fact]
        public async Task ReturnCarAsync_WhenRentalDoesNotExist_ThrowsNotFoundException()
        {
            _rentalRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Rental?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.ReturnCarAsync(1, DateTime.Today));
        }

        [Fact]
        public async Task ReturnCarAsync_WithValidData_UpdatesRentalAndLogs()
        {
            var rentDate = new DateTime(2024, 1, 10);
            var rental = new Rental(1, 1, 1, rentDate);

            _rentalRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(rental);

            var returnDate = rentDate.AddDays(1);

            await _sut.ReturnCarAsync(1, returnDate);

            _rentalRepositoryMock.Verify(r => r.UpdateAsync(rental), Times.Once);
            Assert.Equal(returnDate, rental.ReturnDate);

            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("Car returned"))),
                Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            _rentalRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Rental>
                {
                    new Rental(1, 1, 1, DateTime.Today)
                });

            var result = await _sut.GetAllAsync();

            var dto = Assert.Single(result);
            Assert.Equal(1, dto.CustomerId);
            Assert.Equal(1, dto.CarId);
        }

        [Fact]
        public async Task GetActiveAsync_ReturnsMappedDtos()
        {
            _rentalRepositoryMock
                .Setup(r => r.GetActiveRentalsAsync())
                .ReturnsAsync(new List<Rental>
                {
                    new Rental(1, 1, 1, DateTime.Today)
                });

            var result = await _sut.GetActiveAsync();

            var dto = Assert.Single(result);
            Assert.True(dto.IsActive);
        }
    }
}

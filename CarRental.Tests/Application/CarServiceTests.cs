using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.Exceptions;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Services;
using CarRental.Domain.Entities;
using Moq;
using Xunit;

namespace CarRental.Tests.Application
{
    public class CarServiceTests
    {
        private readonly Mock<ICarRepository> _repositoryMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly CarService _sut;

        public CarServiceTests()
        {
            _repositoryMock = new Mock<ICarRepository>();
            _loggerMock = new Mock<ILogger>();
            _sut = new CarService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_WhenCarsExist_ReturnsMappedDtos()
        {
            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Car>
                {
                    new Car(1, "Toyota", "Corolla", 2020, "VIN1")
                });

            var result = await _sut.GetAllAsync();

            var car = Assert.Single(result);
            Assert.Equal(1, car.Id);
            Assert.Equal("Toyota", car.Brand);
            Assert.Equal("Corolla", car.Model);
            Assert.Equal(2020, car.Year);
            Assert.Equal("VIN1", car.Vin);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCarDoesNotExist_ThrowsNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Car?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(1));
        }

        [Fact]
        public async Task AddAsync_WithInvalidInput_ThrowsValidationExceptionAndDoesNotCallRepository()
        {
            await Assert.ThrowsAsync<ValidationException>(() => _sut.AddAsync("", "", 0, ""));

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Car>()), Times.Never);
        }

        [Fact]
        public async Task AddAsync_WithValidInput_PersistsCarAndLogs()
        {
            Car? addedCar = null;

            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Car>()))
                .Callback<Car>(c => addedCar = c)
                .Returns(Task.CompletedTask);

            var result = await _sut.AddAsync("Toyota", "Corolla", 2020, "VIN1");

            Assert.NotNull(addedCar);
            Assert.Equal(addedCar!.Id, result.Id);
            Assert.Equal("Toyota", result.Brand);
            Assert.Equal("Corolla", result.Model);
            Assert.Equal(2020, result.Year);
            Assert.Equal("VIN1", result.Vin);

            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("Car added"))),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenCarDoesNotExist_ThrowsNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Car?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.UpdateAsync(1, "Toyota", "Corolla", 2020, "VIN1"));
        }

        [Fact]
        public async Task DeleteAsync_WhenCarDoesNotExist_ThrowsNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Car?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(1));
        }

        [Fact]
        public async Task DeleteAsync_WhenCarExists_DeletesAndLogs()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Car(1, "Toyota", "Corolla", 2020, "VIN1"));

            await _sut.DeleteAsync(1);

            _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("Car deleted"))),
                Times.Once);
        }

        [Fact]
        public async Task SearchByBrandAsync_WithEmptyBrand_ReturnsEmptyCollectionAndDoesNotHitRepository()
        {
            var result = await _sut.SearchByBrandAsync("   ");

            Assert.Empty(result);
            _repositoryMock.Verify(r => r.SearchByBrandAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SearchByBrandAsync_WithBrand_ReturnsMappedDtos()
        {
            _repositoryMock
                .Setup(r => r.SearchByBrandAsync("Toyota"))
                .ReturnsAsync(new List<Car>
                {
                    new Car(1, "Toyota", "Corolla", 2020, "VIN1")
                });

            var result = await _sut.SearchByBrandAsync("Toyota");

            var car = Assert.Single(result);
            Assert.Equal(1, car.Id);
            Assert.Equal("Toyota", car.Brand);
        }
    }
}

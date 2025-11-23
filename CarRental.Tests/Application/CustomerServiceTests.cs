using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly CustomerService _sut;

        public CustomerServiceTests()
        {
            _repositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger>();
            _sut = new CustomerService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_WhenCustomersExist_ReturnsMappedDtos()
        {
            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Customer>
                {
                    new Customer(1, "Alice", Email.Create("alice@example.com"))
                });

            var result = await _sut.GetAllAsync();

            var customer = Assert.Single(result);
            Assert.Equal(1, customer.Id);
            Assert.Equal("Alice", customer.Name);
            Assert.Equal("alice@example.com", customer.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Customer?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(1));
        }

        [Fact]
        public async Task AddAsync_WithInvalidInput_ThrowsValidationExceptionAndDoesNotCallRepository()
        {
            await Assert.ThrowsAsync<ValidationException>(() => _sut.AddAsync("", ""));

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task AddAsync_WithValidInput_PersistsCustomerAndLogs()
        {
            Customer? addedCustomer = null;

            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Customer>()))
                .Callback<Customer>(c => addedCustomer = c)
                .Returns(Task.CompletedTask);

            var result = await _sut.AddAsync("Alice", "alice@example.com");

            Assert.NotNull(addedCustomer);
            Assert.Equal(addedCustomer!.Id, result.Id);
            Assert.Equal("Alice", result.Name);
            Assert.Equal("alice@example.com", result.Email);

            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("Customer added"))),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Customer?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.UpdateAsync(1, "Alice", "alice@example.com"));
        }

        [Fact]
        public async Task DeleteAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Customer?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(1));
        }

        [Fact]
        public async Task DeleteAsync_WhenCustomerExists_DeletesAndLogs()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Customer(1, "Alice", Email.Create("alice@example.com")));

            await _sut.DeleteAsync(1);

            _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("Customer deleted"))),
                Times.Once);
        }

        [Fact]
        public async Task SearchByNameAsync_WithEmptyName_ReturnsEmptyCollectionAndDoesNotHitRepository()
        {
            var result = await _sut.SearchByNameAsync("   ");

            Assert.Empty(result);
            _repositoryMock.Verify(r => r.SearchByNameAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SearchByNameAsync_WithName_ReturnsMappedDtos()
        {
            _repositoryMock
                .Setup(r => r.SearchByNameAsync("Alice"))
                .ReturnsAsync(new List<Customer>
                {
                    new Customer(1, "Alice", Email.Create("alice@example.com"))
                });

            var result = await _sut.SearchByNameAsync("Alice");

            var customer = Assert.Single(result);
            Assert.Equal(1, customer.Id);
            Assert.Equal("Alice", customer.Name);
            Assert.Equal("alice@example.com", customer.Email);
        }
    }
}

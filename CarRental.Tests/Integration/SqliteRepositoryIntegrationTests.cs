using System;
using System.IO;
using System.Threading.Tasks;
using CarRental.Domain.Entities;
using CarRental.Domain.ValueObjects;
using CarRental.Infrastructure.Configuration;
using CarRental.Infrastructure.Persistence;
using CarRental.Infrastructure.Persistence.Migrations;
using CarRental.Infrastructure.Persistence.Repositories;
using CarRental.Application.Interfaces.Infrastructure;
using Microsoft.Data.Sqlite;
using Xunit;

namespace CarRental.Tests.Integration
{
    public class SqliteRepositoryIntegrationTests
    {
        [Fact]
        public async Task DatabaseInitializer_CreatesSchemaAndSeedsAdminUser()
        {
            var dbPath = Path.Combine(Path.GetTempPath(), $"CarRentalTest_Admin_{Guid.NewGuid():N}.db");
            try
            {
                var connectionFactory = await CreateDatabaseAsync(dbPath);

                var userRepository = new SQLiteUserRepository(connectionFactory);

                var admin = await userRepository.GetByUsernameAsync("admin");

                Assert.NotNull(admin);
                Assert.Equal("admin", admin!.Username);
                Assert.Equal("admin@example.com", admin.Email.Value);
            }
            finally
            {
                DeleteIfExists(dbPath);
            }
        }

        [Fact]
        public async Task CustomerRepository_CanAddAndRetrieveCustomer()
        {
            var dbPath = Path.Combine(Path.GetTempPath(), $"CarRentalTest_Customer_{Guid.NewGuid():N}.db");
            try
            {
                var connectionFactory = await CreateDatabaseAsync(dbPath);
                var customerRepository = new SQLiteCustomerRepository(connectionFactory);

                var customer = new Customer("Test User", Email.Create("test@example.com"));

                await customerRepository.AddAsync(customer);

                var loaded = await customerRepository.GetByIdAsync(customer.Id);

                Assert.NotNull(loaded);
                Assert.Equal(customer.Id, loaded!.Id);
                Assert.Equal("Test User", loaded.Name);
                Assert.Equal("test@example.com", loaded.Email.Value);

                var all = await customerRepository.GetAllAsync();
                Assert.Contains(all, c => c.Id == customer.Id);
            }
            finally
            {
                DeleteIfExists(dbPath);
            }
        }

        [Fact]
        public async Task CarRepository_CanAddSearchAndDeleteCar()
        {
            var dbPath = Path.Combine(Path.GetTempPath(), $"CarRentalTest_Car_{Guid.NewGuid():N}.db");
            try
            {
                var connectionFactory = await CreateDatabaseAsync(dbPath);
                var carRepository = new SQLiteCarRepository(connectionFactory);

                var car = new Car("Toyota", "Corolla", 2020, "VIN-INTEG-1");

                await carRepository.AddAsync(car);

                var loaded = await carRepository.GetByIdAsync(car.Id);
                Assert.NotNull(loaded);
                Assert.Equal("Toyota", loaded!.Brand);
                Assert.Equal("Corolla", loaded.Model);

                var toyotaCars = await carRepository.SearchByBrandAsync("Toyota");
                Assert.Contains(toyotaCars, c => c.Id == car.Id);

                await carRepository.DeleteAsync(car.Id);
                var removed = await carRepository.GetByIdAsync(car.Id);
                Assert.Null(removed);
            }
            finally
            {
                DeleteIfExists(dbPath);
            }
        }

        [Fact]
        public async Task RentalRepository_CanAddAndCompleteRental()
        {
            var dbPath = Path.Combine(Path.GetTempPath(), $"CarRentalTest_Rental_{Guid.NewGuid():N}.db");
            try
            {
                var connectionFactory = await CreateDatabaseAsync(dbPath);

                var customerRepository = new SQLiteCustomerRepository(connectionFactory);
                var carRepository = new SQLiteCarRepository(connectionFactory);
                var rentalRepository = new SQLiteRentalRepository(connectionFactory);

                // Arrange: add customer and car
                var customer = new Customer("Rental User", Email.Create("rental@example.com"));
                await customerRepository.AddAsync(customer);

                var car = new Car("Honda", "Civic", 2021, "VIN-INTEG-2");
                await carRepository.AddAsync(car);

                var rentDate = new DateTime(2024, 1, 10);
                var rental = new Rental(customer.Id, car.Id, rentDate);

                // Act: add rental
                await rentalRepository.AddAsync(rental);

                var activeRentals = await rentalRepository.GetActiveRentalsAsync();
                Assert.Contains(activeRentals, r => r.Id == rental.Id && r.IsActive);

                // Return car
                var returnDate = rentDate.AddDays(3);
                rental.Return(returnDate);
                await rentalRepository.UpdateAsync(rental);

                var reloaded = await rentalRepository.GetByIdAsync(rental.Id);
                Assert.NotNull(reloaded);
                Assert.False(reloaded!.IsActive);
                Assert.Equal(returnDate, reloaded.ReturnDate);
            }
            finally
            {
                DeleteIfExists(dbPath);
            }
        }

        // --- helpers -------------------------------------------------------------------------

        private static async Task<SQLiteDbConnectionFactory> CreateDatabaseAsync(string dbPath)
        {
            // Configure connection string via environment variable (used by AppConfiguration)
            Environment.SetEnvironmentVariable("CAR_RENTAL_CONNECTION_STRING", $"Data Source={dbPath};");

            var config = new AppConfiguration();
            var connectionFactory = new SQLiteDbConnectionFactory(config);

            var logger = new TestLogger();
            var initializer = new DatabaseInitializer(connectionFactory, logger);
            await initializer.InitializeAsync();

            return connectionFactory;
        }

        private static void DeleteIfExists(string path)
        {
            try
            {
                // Release SQLite file handles just in case some pooled connections are still around
                SqliteConnection.ClearAllPools();

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (IOException)
            {
                // We do not want cleanup issues to fail tests.
                // Temp files will be cleaned up by the OS eventually.
            }
        }

        private sealed class TestLogger : ILogger
        {
            public void Info(string message) { }

            public void Warning(string message) { }

            public void Error(string message, Exception? exception = null) { }
        }
    }
}

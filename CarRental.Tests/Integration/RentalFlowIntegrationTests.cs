using System;
using System.IO;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Application.Services;
using CarRental.Domain.ValueObjects;
using CarRental.Infrastructure.Configuration;
using CarRental.Infrastructure.Persistence;
using CarRental.Infrastructure.Persistence.Migrations;
using CarRental.Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Xunit;

namespace CarRental.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class RentalFlowIntegrationTests
    {
        [Fact]
        public async Task FullRentalFlow_WorksCorrectly_WithRealServicesAndSqlite()
        {
            var dbPath = Path.Combine(
                Path.GetTempPath(),
                $"CarRentalTest_Flow_{Guid.NewGuid():N}.db");

            try
            {
                var connectionFactory = await CreateDatabaseAsync(dbPath);

                // real repositories
                var customerRepository = new SQLiteCustomerRepository(connectionFactory);
                var carRepository = new SQLiteCarRepository(connectionFactory);
                var rentalRepository = new SQLiteRentalRepository(connectionFactory);

                // simple no-op logger implementation
                var logger = new TestLogger();

                // real services wired with real repositories
                var customerService = new CustomerService(customerRepository, logger);
                var carService = new CarService(carRepository, logger);
                var rentalService = new RentalService(rentalRepository, carRepository, customerRepository, logger);
                var dashboardService = new DashboardService(carRepository, customerRepository, rentalRepository);

                // 1) add customer
                var customerDto = await customerService.AddAsync("Alice Smith", "alice@example.com");

                // 2) add car
                var carDto = await carService.AddAsync("Toyota", "Corolla", 2020, "VIN-FLOW-1");

                // 3) rent car
                var rentDate = new DateTime(2024, 1, 10);
                var rentalDto = await rentalService.RentCarAsync(customerDto.Id, carDto.Id, rentDate);

                // after rent: dashboard should see 1 active rental, 1 car, 1 customer
                var summaryAfterRent = await dashboardService.GetSummaryAsync();
                Assert.Equal(1, summaryAfterRent.ActiveRentals);
                Assert.Equal(1, summaryAfterRent.TotalCars);
                Assert.Equal(1, summaryAfterRent.TotalCustomers);

                // 4) return car
                var returnDate = rentDate.AddDays(3);
                await rentalService.ReturnCarAsync(rentalDto.Id, returnDate);

                // after return: no active rentals
                var summaryAfterReturn = await dashboardService.GetSummaryAsync();
                Assert.Equal(0, summaryAfterReturn.ActiveRentals);

                // verify rental details from service
                var allRentals = await rentalService.GetAllAsync();
                var rental = Assert.Single(allRentals);

                Assert.False(rental.IsActive);
                Assert.Equal(returnDate, rental.ReturnDate);
                Assert.Equal(customerDto.Id, rental.CustomerId);
                Assert.Equal(carDto.Id, rental.CarId);
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
            Environment.SetEnvironmentVariable(
                "CAR_RENTAL_CONNECTION_STRING",
                $"Data Source={dbPath};");

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
                // Do not fail tests because of cleanup issues.
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

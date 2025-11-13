using System.Collections.Generic;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Interfaces.Services;
using CarRental.Application.Services;
using CarRental.ConsoleUI.Authentication;
using CarRental.ConsoleUI.Commands;
using CarRental.ConsoleUI.Commands.Car;
using CarRental.ConsoleUI.Commands.Customer;
using CarRental.ConsoleUI.Commands.Dashboard;
using CarRental.ConsoleUI.Commands.Rental;
using CarRental.ConsoleUI.Commands.System;
using CarRental.ConsoleUI.Input;
using CarRental.Infrastructure.Configuration;
using CarRental.Infrastructure.Logging;
using CarRental.Infrastructure.Persistence;
using CarRental.Infrastructure.Persistence.Migrations;
using CarRental.Infrastructure.Persistence.Repositories;
using CarRental.Infrastructure.Security;

namespace CarRental.ConsoleUI.CompositionRoot
{
    /// <summary>
    /// Manual dependency injection / composition root.
    /// Wires together all layers.
    /// </summary>
    public class Bootstrapper
    {
        public ApplicationContext Build()
        {
            // Configuration
            var configuration = new AppConfiguration();

            // Infrastructure
            IDbConnectionFactory dbConnectionFactory = new SQLiteDbConnectionFactory(configuration);
            ILogger logger = new FileLogger(configuration);
            var passwordHasher = new PasswordHasher();
            var dbInitializer = new DatabaseInitializer(dbConnectionFactory, logger);

            // Repositories
            ICarRepository carRepository = new SQLiteCarRepository(dbConnectionFactory);
            ICustomerRepository customerRepository = new SQLiteCustomerRepository(dbConnectionFactory);
            IRentalRepository rentalRepository = new SQLiteRentalRepository(dbConnectionFactory);
            IUserRepository userRepository = new SQLiteUserRepository(dbConnectionFactory);

            // Application services
            ICarService carService = new CarService(carRepository, logger);
            ICustomerService customerService = new CustomerService(customerRepository, logger);
            IRentalService rentalService = new RentalService(rentalRepository, carRepository, customerRepository, logger);
            IAuthService authService = new AuthService(userRepository, passwordHasher, logger);
            IDashboardService dashboardService = new DashboardService(carRepository, customerRepository, rentalRepository);

            // UI helpers
            var inputValidator = new InputValidator();
            var loginScreen = new LoginScreen(authService, inputValidator);

            // Commands (Command pattern)
            var commands = new List<IMenuCommand>
            {
                // Car
                new AddCarCommand(inputValidator, carService),
                new EditCarCommand(inputValidator, carService),
                new DeleteCarCommand(inputValidator, carService),
                new ListCarsCommand(carService),
                new SearchCarsCommand(inputValidator, carService),

                // Customer
                new AddCustomerCommand(inputValidator, customerService),
                new EditCustomerCommand(inputValidator, customerService),
                new DeleteCustomerCommand(inputValidator, customerService),
                new ListCustomersCommand(customerService),
                new SearchCustomersCommand(inputValidator, customerService),

                // Rental
                new RentCarCommand(inputValidator, rentalService),
                new ReturnCarCommand(inputValidator, rentalService),
                new ListRentalsCommand(rentalService),

                // Dashboard
                new ShowDashboardCommand(dashboardService),

                // System
                new ExitApplicationCommand()
            };

            return new ApplicationContext(
                dbInitializer,
                inputValidator,
                loginScreen,
                commands);
        }
    }

    /// <summary>
    /// Simple container object holding references to core services
    /// needed by the console UI.
    /// </summary>
    public sealed class ApplicationContext
    {
        public DatabaseInitializer DatabaseInitializer { get; }
        public InputValidator InputValidator { get; }
        public LoginScreen LoginScreen { get; }
        public IReadOnlyList<IMenuCommand> Commands { get; }

        public ApplicationContext(
            DatabaseInitializer databaseInitializer,
            InputValidator inputValidator,
            LoginScreen loginScreen,
            IReadOnlyList<IMenuCommand> commands)
        {
            DatabaseInitializer = databaseInitializer;
            InputValidator = inputValidator;
            LoginScreen = loginScreen;
            Commands = commands;
        }
    }
}

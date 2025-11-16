using System;
using System.Data.Common;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Infrastructure;

namespace CarRental.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Creates database schema if it does not exist and seeds initial data.
    /// </summary>
    public class DatabaseInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;

        public DatabaseInitializer(IDbConnectionFactory connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
                await CreateTablesAsync(connection);
                await SeedInitialDataAsync(connection);
                _logger.Info("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error("Database initialization failed.", ex);
                throw;
            }
        }

        private static async Task CreateTablesAsync(DbConnection connection)
        {
            var sql = @"
CREATE TABLE IF NOT EXISTS Cars (
    Id    INTEGER PRIMARY KEY AUTOINCREMENT,
    Brand TEXT NOT NULL,
    Model TEXT NOT NULL,
    Year  INTEGER NOT NULL,
    Vin   TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Customers (
    Id    INTEGER PRIMARY KEY AUTOINCREMENT,
    Name  TEXT NOT NULL,
    Email TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Rentals (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerId  INTEGER NOT NULL,
    CarId       INTEGER NOT NULL,
    RentDate    TEXT NOT NULL,
    ReturnDate  TEXT NULL,
    FOREIGN KEY (CustomerId) REFERENCES Customers (Id),
    FOREIGN KEY (CarId)      REFERENCES Cars (Id)
);

CREATE TABLE IF NOT EXISTS Users (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    Username     TEXT NOT NULL UNIQUE,
    Email        TEXT NOT NULL,
    PasswordHash TEXT NOT NULL,
    Salt         TEXT NOT NULL,
    Role         INTEGER NOT NULL
);
";
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync();
        }

        private static async Task SeedInitialDataAsync(DbConnection connection)
        {
            // Seed one admin user if table is empty

            // Check if any users exist
            await using (var checkCmd = connection.CreateCommand())
            {
                checkCmd.CommandText = "SELECT COUNT(1) FROM Users;";
                var result = await checkCmd.ExecuteScalarAsync();
                var count = Convert.ToInt32(result);

                if (count > 0)
                {
                    return;
                }
            }

            // We cannot access PasswordHasher here (that's application-level dependency),
            // so the admin user should be seeded from the composition root (ConsoleUI)
            // or you can insert a placeholder and force password reset.
            //
            // Seed default admin user with a real hashed password (password: admin123)
            await using (var insertCmd = connection.CreateCommand())
            {
                insertCmd.CommandText = @"
INSERT INTO Users (Username, Email, PasswordHash, Salt, Role)
VALUES (
    'admin',
    'admin@example.com',
    'tuuLxfgxCnjP/ODFe3AVvjJmwfpgEuFzpTNKJSHGTMs=',
    'rZToGkrZ/0m/0Q8tPwkcNg==',
    1
);";
                await insertCmd.ExecuteNonQueryAsync();
            }

        }
    }
}

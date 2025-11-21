using System;
using System.Data.Common;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Infrastructure;

namespace CarRental.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Creates database schema if it does not exist and seeds initial data.
    /// Uses explicit transaction to ensure ACID properties.
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
                await using var transaction = await connection.BeginTransactionAsync();

                await CreateTablesAsync(connection, transaction);
                await CreateIndexesAndViewsAsync(connection, transaction);
                await SeedInitialDataAsync(connection, transaction);

                await transaction.CommitAsync();

                _logger.Info("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error("Database initialization failed.", ex);
                throw;
            }
        }

        private static async Task CreateTablesAsync(DbConnection connection, DbTransaction transaction)
        {
            const string sql = @"
CREATE TABLE IF NOT EXISTS Cars (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    Brand     TEXT NOT NULL,
    Model     TEXT NOT NULL,
    Year      INTEGER NOT NULL,
    Vin       TEXT NOT NULL,
    IsActive  INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS Customers (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    Name      TEXT NOT NULL,
    Email     TEXT NOT NULL,
    IsActive  INTEGER NOT NULL DEFAULT 1
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
    Role         INTEGER NOT NULL,
    IsActive     INTEGER NOT NULL DEFAULT 1
);";

            await using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Creates indexes and views to improve query performance and demonstrate advanced DB features.
        /// </summary>
        private static async Task CreateIndexesAndViewsAsync(DbConnection connection, DbTransaction transaction)
        {
            const string sql = @"
-- Indexes for faster lookups and filtering
CREATE INDEX IF NOT EXISTS IX_Cars_IsActive_Brand_Model
    ON Cars (IsActive, Brand, Model);

CREATE INDEX IF NOT EXISTS IX_Customers_IsActive_Name
    ON Customers (IsActive, Name);

CREATE INDEX IF NOT EXISTS IX_Rentals_CustomerId
    ON Rentals (CustomerId);

CREATE INDEX IF NOT EXISTS IX_Rentals_CarId
    ON Rentals (CarId);

CREATE INDEX IF NOT EXISTS IX_Users_IsActive_Username
    ON Users (IsActive, Username);

-- Partial index for active rentals (SQLite supports filtered indexes)
CREATE INDEX IF NOT EXISTS IX_Rentals_ActiveByCar
    ON Rentals (CarId)
    WHERE ReturnDate IS NULL;

-- View for active rentals
CREATE VIEW IF NOT EXISTS ActiveRentalsView AS
SELECT
    r.Id,
    r.CustomerId,
    r.CarId,
    r.RentDate,
    r.ReturnDate
FROM Rentals AS r
WHERE r.ReturnDate IS NULL;";

            await using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Seeds initial data (default admin user) in a transactional way.
        /// </summary>
        private static async Task SeedInitialDataAsync(DbConnection connection, DbTransaction transaction)
        {
            // Check if any users exist
            await using (var checkCmd = connection.CreateCommand())
            {
                checkCmd.Transaction = transaction;
                checkCmd.CommandText = "SELECT COUNT(1) FROM Users;";
                var result = await checkCmd.ExecuteScalarAsync();
                var count = Convert.ToInt32(result);

                if (count > 0)
                {
                    // Users already exist – nothing to seed
                    return;
                }
            }

            // Seed default admin user with a real hashed password (password: admin123)
            await using (var insertCmd = connection.CreateCommand())
            {
                insertCmd.Transaction = transaction;
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

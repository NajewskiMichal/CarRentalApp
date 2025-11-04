using System;
using System.Data.SQLite;

namespace CarRentalConsoleApp
{
    public static class DatabaseHelper
    {
        private static string connectionString = "Data Source=CarRental.db;Version=3;";

        public static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createCustomersTable = @"
                    CREATE TABLE IF NOT EXISTS Customers (
                        CustomerId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Email TEXT NOT NULL UNIQUE
                    );";

                string createCarsTable = @"
                    CREATE TABLE IF NOT EXISTS Cars (
                        CarId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Brand TEXT NOT NULL,
                        Model TEXT NOT NULL,
                        Year INTEGER NOT NULL,
                        VIN TEXT NOT NULL UNIQUE
                    );";

                string createRentalsTable = @"
                    CREATE TABLE IF NOT EXISTS Rentals (
                        RentalId INTEGER PRIMARY KEY AUTOINCREMENT,
                        CustomerId INTEGER NOT NULL,
                        CarId INTEGER NOT NULL,
                        RentDate TEXT NOT NULL,
                        ReturnDate TEXT,
                        FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
                        FOREIGN KEY (CarId) REFERENCES Cars(CarId)
                    );";

                ExecuteNonQuery(connection, createCustomersTable);
                ExecuteNonQuery(connection, createCarsTable);
                ExecuteNonQuery(connection, createRentalsTable);
            }
        }

        public static void ExecuteNonQuery(SQLiteConnection connection, string query, SQLiteParameter[] parameters = null)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.ExecuteNonQuery();
            }
        }

        public static SQLiteDataReader ExecuteQuery(SQLiteConnection connection, string query, SQLiteParameter[] parameters = null)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteReader();
            }
        }

        public static SQLiteConnection GetConnection()
        {
            var connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
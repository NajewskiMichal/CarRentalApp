using System;

namespace CarRental.Infrastructure.Configuration
{
    public class AppConfiguration
    {
        public string ConnectionString { get; }
        public string LogFilePath { get; }

        public AppConfiguration()
        {
 
            ConnectionString =
                Environment.GetEnvironmentVariable("CAR_RENTAL_CONNECTION_STRING")
                ?? "Data Source=CarRental.db;";

            LogFilePath =
                Environment.GetEnvironmentVariable("CAR_RENTAL_LOG_FILE")
                ?? "CarRental.log";
        }
    }
}

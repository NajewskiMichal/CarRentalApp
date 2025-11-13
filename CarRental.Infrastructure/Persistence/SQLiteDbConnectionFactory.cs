using System.Data.Common;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Infrastructure.Configuration;
using Microsoft.Data.Sqlite;

namespace CarRental.Infrastructure.Persistence
{
    public class SQLiteDbConnectionFactory : IDbConnectionFactory
    {
        private readonly AppConfiguration _configuration;

        public SQLiteDbConnectionFactory(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<DbConnection> CreateOpenConnectionAsync()
        {
            var connection = new SqliteConnection(_configuration.ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}

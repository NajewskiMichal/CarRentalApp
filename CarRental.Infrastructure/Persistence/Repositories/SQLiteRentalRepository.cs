using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Domain.Entities;

namespace CarRental.Infrastructure.Persistence.Repositories
{
    public class SQLiteRentalRepository : IRentalRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SQLiteRentalRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Rental?> GetByIdAsync(int id)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT Id, CustomerId, CarId, RentDate, ReturnDate
FROM Rentals
WHERE Id = @Id;";
            AddParameter(cmd, "@Id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapRental(reader);
            }

            return null;
        }

        public async Task<IReadOnlyList<Rental>> GetAllAsync()
        {
            var result = new List<Rental>();
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT Id, CustomerId, CarId, RentDate, ReturnDate
FROM Rentals
ORDER BY RentDate DESC;";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapRental(reader));
            }

            return result;
        }

        public async Task AddAsync(Rental entity)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
INSERT INTO Rentals (CustomerId, CarId, RentDate, ReturnDate)
VALUES (@CustomerId, @CarId, @RentDate, @ReturnDate);
SELECT last_insert_rowid();";

            AddParameter(cmd, "@CustomerId", entity.CustomerId);
            AddParameter(cmd, "@CarId", entity.CarId);
            AddParameter(cmd, "@RentDate", entity.RentDate.ToString("o"));
            AddParameter(cmd, "@ReturnDate", entity.ReturnDate != null ? entity.ReturnDate.Value.ToString("o") : (object)DBNull.Value);

            var idObj = await cmd.ExecuteScalarAsync();
            var id = Convert.ToInt32(idObj);
            EntityIdSetter.SetId(entity, id);
        }

        public async Task UpdateAsync(Rental entity)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
UPDATE Rentals
SET CustomerId = @CustomerId,
    CarId = @CarId,
    RentDate = @RentDate,
    ReturnDate = @ReturnDate
WHERE Id = @Id;";

            AddParameter(cmd, "@CustomerId", entity.CustomerId);
            AddParameter(cmd, "@CarId", entity.CarId);
            AddParameter(cmd, "@RentDate", entity.RentDate.ToString("o"));
            AddParameter(cmd, "@ReturnDate", entity.ReturnDate != null ? entity.ReturnDate.Value.ToString("o") : (object)DBNull.Value);
            AddParameter(cmd, "@Id", entity.Id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Rentals WHERE Id = @Id;";
            AddParameter(cmd, "@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Rental>> GetActiveRentalsAsync()
        {
            var result = new List<Rental>();
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT Id, CustomerId, CarId, RentDate, ReturnDate
FROM Rentals
WHERE ReturnDate IS NULL
ORDER BY RentDate DESC;";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapRental(reader));
            }

            return result;
        }

        private static Rental MapRental(DbDataReader reader)
        {
            var id = reader.GetInt32(0);
            var customerId = reader.GetInt32(1);
            var carId = reader.GetInt32(2);

            var rentDateString = reader.GetString(3);
            var rentDate = DateTime.Parse(rentDateString, null, System.Globalization.DateTimeStyles.RoundtripKind);

            DateTime? returnDate = null;
            if (!reader.IsDBNull(4))
            {
                var returnDateString = reader.GetString(4);
                returnDate = DateTime.Parse(returnDateString, null, System.Globalization.DateTimeStyles.RoundtripKind);
            }

            return new Rental(id, customerId, carId, rentDate, returnDate);
        }

        private static void AddParameter(DbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}

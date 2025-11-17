using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Domain.Entities;

namespace CarRental.Infrastructure.Persistence.Repositories
{
    public class SQLiteCarRepository : ICarRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SQLiteCarRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Brand, Model, Year, Vin FROM Cars WHERE Id = @Id AND IsActive = 1;";
            AddParameter(cmd, "@Id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapCar(reader);
            }

            return null;
        }

        public async Task<IReadOnlyList<Car>> GetAllAsync()
        {
            var result = new List<Car>();
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Brand, Model, Year, Vin FROM Cars WHERE IsActive = 1 ORDER BY Brand, Model;";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapCar(reader));
            }

            return result;
        }

        public async Task AddAsync(Car entity)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
INSERT INTO Cars (Brand, Model, Year, Vin)
VALUES (@Brand, @Model, @Year, @Vin);
SELECT last_insert_rowid();";

            AddParameter(cmd, "@Brand", entity.Brand);
            AddParameter(cmd, "@Model", entity.Model);
            AddParameter(cmd, "@Year", entity.Year);
            AddParameter(cmd, "@Vin", entity.Vin);

            var idObj = await cmd.ExecuteScalarAsync();
            var id = Convert.ToInt32(idObj);
            EntityIdSetter.SetId(entity, id);
        }

        public async Task UpdateAsync(Car entity)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
UPDATE Cars
SET Brand = @Brand, Model = @Model, Year = @Year, Vin = @Vin
WHERE Id = @Id;";

            AddParameter(cmd, "@Brand", entity.Brand);
            AddParameter(cmd, "@Model", entity.Model);
            AddParameter(cmd, "@Year", entity.Year);
            AddParameter(cmd, "@Vin", entity.Vin);
            AddParameter(cmd, "@Id", entity.Id);

            await cmd.ExecuteNonQueryAsync();
        }

        
        public async Task DeleteAsync(int id)
        {
            await SetActiveAsync(id, false);
        }

        public async Task<IReadOnlyList<Car>> SearchByBrandAsync(string brand)
        {
            var result = new List<Car>();
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT Id, Brand, Model, Year, Vin
FROM Cars
WHERE Brand LIKE @Brand AND IsActive = 1
ORDER BY Brand, Model;";
            AddParameter(cmd, "@Brand", $"%{brand}%");

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapCar(reader));
            }

            return result;
        }

        

        public async Task<IReadOnlyList<Car>> GetAllIncludingInactiveAsync()
        {
            var result = new List<Car>();
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Brand, Model, Year, Vin FROM Cars ORDER BY IsActive DESC, Brand, Model;";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapCar(reader));
            }

            return result;
        }

        public async Task<IReadOnlyList<Car>> GetInactiveAsync()
        {
            var result = new List<Car>();
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Brand, Model, Year, Vin FROM Cars WHERE IsActive = 0 ORDER BY Brand, Model;";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapCar(reader));
            }

            return result;
        }

        public async Task SetActiveAsync(int id, bool isActive)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Cars SET IsActive = @IsActive WHERE Id = @Id;";
            AddParameter(cmd, "@Id", id);
            AddParameter(cmd, "@IsActive", isActive ? 1 : 0);
            await cmd.ExecuteNonQueryAsync();
        }

        // --- helpers ---

        private static Car MapCar(DbDataReader reader)
        {
            var id = reader.GetInt32(0);
            var brand = reader.GetString(1);
            var model = reader.GetString(2);
            var year = reader.GetInt32(3);
            var vin = reader.GetString(4);

            return new Car(id, brand, model, year, vin);
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

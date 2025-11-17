using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.ValueObjects;

namespace CarRental.Infrastructure.Persistence.Repositories
{
    public class SQLiteCustomerRepository : ICustomerRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SQLiteCustomerRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Email FROM Customers WHERE Id = @Id AND IsActive = 1;";
            AddParameter(cmd, "@Id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapCustomer(reader);
            }

            return null;
        }

        public async Task<IReadOnlyList<Customer>> GetAllAsync()
        {
            var result = new List<Customer>();
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Email FROM Customers WHERE IsActive = 1 ORDER BY Name;";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapCustomer(reader));
            }

            return result;
        }

        public async Task AddAsync(Customer entity)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
INSERT INTO Customers (Name, Email)
VALUES (@Name, @Email);
SELECT last_insert_rowid();";

            AddParameter(cmd, "@Name", entity.Name);
            AddParameter(cmd, "@Email", entity.Email.Value);

            var idObj = await cmd.ExecuteScalarAsync();
            var id = Convert.ToInt32(idObj);
            EntityIdSetter.SetId(entity, id);
        }

        public async Task UpdateAsync(Customer entity)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
UPDATE Customers
SET Name = @Name, Email = @Email
WHERE Id = @Id;";

            AddParameter(cmd, "@Name", entity.Name);
            AddParameter(cmd, "@Email", entity.Email.Value);
            AddParameter(cmd, "@Id", entity.Id);

            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Soft delete: klient staje się archiwalny (IsActive = 0).
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await SetActiveAsync(id, false);
        }

        public async Task<IReadOnlyList<Customer>> SearchByNameAsync(string name)
        {
            var result = new List<Customer>();
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT Id, Name, Email 
FROM Customers
WHERE Name LIKE @Name AND IsActive = 1
ORDER BY Name;";
            AddParameter(cmd, "@Name", $"%{name}%");

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapCustomer(reader));
            }

            return result;
        }

        // --- NOWE METODY DLA ADMINA ---

        public async Task<IReadOnlyList<Customer>> GetAllIncludingInactiveAsync()
        {
            var result = new List<Customer>();
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Email FROM Customers ORDER BY IsActive DESC, Name;";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapCustomer(reader));
            }

            return result;
        }

        public async Task<IReadOnlyList<Customer>> GetInactiveAsync()
        {
            var result = new List<Customer>();
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Email FROM Customers WHERE IsActive = 0 ORDER BY Name;";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapCustomer(reader));
            }

            return result;
        }

        public async Task SetActiveAsync(int id, bool isActive)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Customers SET IsActive = @IsActive WHERE Id = @Id;";
            AddParameter(cmd, "@Id", id);
            AddParameter(cmd, "@IsActive", isActive ? 1 : 0);
            await cmd.ExecuteNonQueryAsync();
        }

        // --- helpers ---

        private static Customer MapCustomer(DbDataReader reader)
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);
            var email = reader.GetString(2);

            var emailVo = Email.Create(email);
            return new Customer(id, name, emailVo);
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

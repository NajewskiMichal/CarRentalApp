using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.ValueObjects;

namespace CarRental.Infrastructure.Persistence.Repositories
{
    public class SQLiteUserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SQLiteUserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT Id, Username, Email, PasswordHash, Salt, Role
FROM Users
WHERE Id = @Id;";
            AddParameter(cmd, "@Id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapUser(reader);
            }

            return null;
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            var result = new List<User>();
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT Id, Username, Email, PasswordHash, Salt, Role
FROM Users
ORDER BY Username;";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapUser(reader));
            }

            return result;
        }

        public async Task AddAsync(User entity)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
INSERT INTO Users (Username, Email, PasswordHash, Salt, Role)
VALUES (@Username, @Email, @PasswordHash, @Salt, @Role);
SELECT last_insert_rowid();";

            AddParameter(cmd, "@Username", entity.Username);
            AddParameter(cmd, "@Email", entity.Email.Value);
            AddParameter(cmd, "@PasswordHash", entity.PasswordHash);
            AddParameter(cmd, "@Salt", entity.Salt);
            AddParameter(cmd, "@Role", (int)entity.Role);

            var idObj = await cmd.ExecuteScalarAsync();
            var id = Convert.ToInt32(idObj);
            EntityIdSetter.SetId(entity, id);
        }

        public async Task UpdateAsync(User entity)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
UPDATE Users
SET Username = @Username,
    Email = @Email,
    PasswordHash = @PasswordHash,
    Salt = @Salt,
    Role = @Role
WHERE Id = @Id;";

            AddParameter(cmd, "@Username", entity.Username);
            AddParameter(cmd, "@Email", entity.Email.Value);
            AddParameter(cmd, "@PasswordHash", entity.PasswordHash);
            AddParameter(cmd, "@Salt", entity.Salt);
            AddParameter(cmd, "@Role", (int)entity.Role);
            AddParameter(cmd, "@Id", entity.Id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Users WHERE Id = @Id;";
            AddParameter(cmd, "@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT Id, Username, Email, PasswordHash, Salt, Role
FROM Users
WHERE Username = @Username;";
            AddParameter(cmd, "@Username", username);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapUser(reader);
            }

            return null;
        }

        private static User MapUser(DbDataReader reader)
        {
            var id = reader.GetInt32(0);
            var username = reader.GetString(1);
            var email = reader.GetString(2);
            var passwordHash = reader.GetString(3);
            var salt = reader.GetString(4);
            var roleInt = reader.GetInt32(5);
            var role = (UserRole)roleInt;

            var emailVo = Email.Create(email);
            return new User(id, username, emailVo, passwordHash, salt, role);
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

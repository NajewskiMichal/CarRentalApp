using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Application.DTOs;
using CarRental.Application.Exceptions;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Interfaces.Services;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.ValueObjects;

namespace CarRental.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger _logger;

        public UserManagementService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ILogger logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<IReadOnlyList<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto).ToArray();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user is null ? null : MapToDto(user);
        }

        public async Task<UserDto> CreateAsync(string username, string email, string password, UserRole role)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(username))
                errors.Add("Username is required.");

            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Email is required.");

            if (string.IsNullOrWhiteSpace(password))
                errors.Add("Password is required.");

            if (errors.Count > 0)
                throw new ValidationException(errors);

            var existing = await _userRepository.GetByUsernameAsync(username.Trim());
            if (existing != null)
                throw new ValidationException(new[] { "Username is already taken." });

            var emailVo = Email.Create(email.Trim());
            var passwordHash = _passwordHasher.HashPassword(password, out var salt);

            var user = new User(username.Trim(), emailVo, passwordHash, salt, role);
            await _userRepository.AddAsync(user);

            _logger.Info($"User created by admin: {user.Username} ({user.Role})");

            return MapToDto(user);
        }

        public async Task<UserDto> UpdateAsync(int id, string? username, string? email)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("User", id);

            var errors = new List<string>();

            if (username is not null && string.IsNullOrWhiteSpace(username))
                errors.Add("Username cannot be empty.");

            if (email is not null && string.IsNullOrWhiteSpace(email))
                errors.Add("Email cannot be empty.");

            if (errors.Count > 0)
                throw new ValidationException(errors);

            if (!string.IsNullOrWhiteSpace(username))
            {
                user.UpdateUsername(username.Trim());
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailVo = Email.Create(email.Trim());
                user.UpdateEmail(emailVo);
            }

            await _userRepository.UpdateAsync(user);

            _logger.Info($"User updated by admin: {user.Username} ({user.Role})");

            return MapToDto(user);
        }

        public async Task ChangePasswordAsync(int id, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ValidationException(new[] { "Password is required." });

            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("User", id);

            var passwordHash = _passwordHasher.HashPassword(newPassword, out var salt);
            user.UpdatePassword(passwordHash, salt);

            await _userRepository.UpdateAsync(user);

            _logger.Info($"User password changed by admin: {user.Username}");
        }

        public async Task ChangeRoleAsync(int id, UserRole newRole)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("User", id);

            user.ChangeRole(newRole);

            await _userRepository.UpdateAsync(user);

            _logger.Info($"User role changed by admin: {user.Username} -> {newRole}");
        }

        public async Task PromoteToAdminAsync(int id)
        {
            await ChangeRoleAsync(id, UserRole.Admin);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("User", id);

            await _userRepository.DeleteAsync(id);

            _logger.Info($"User deleted by admin: {user.Username}");
        }

        private static UserDto MapToDto(User user) =>
            new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email.Value,
                Role = user.Role
            };
    }
}

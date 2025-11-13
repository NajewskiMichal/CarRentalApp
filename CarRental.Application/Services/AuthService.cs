using System.Collections.Generic;
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
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger _logger;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ILogger logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<UserDto> RegisterAsync(string username, string email, string password, UserRole role)
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
            if (existing is not null)
                throw new ValidationException(new[] { "Username is already taken." });

            var emailVo = Email.Create(email);
            var passwordHash = _passwordHasher.HashPassword(password, out var salt);

            var user = new User(username.Trim(), emailVo, passwordHash, salt, role);
            await _userRepository.AddAsync(user);

            _logger.Info($"User registered: {user.Username} ({user.Role})");

            return MapToDto(user);
        }

        public async Task<UserDto?> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _userRepository.GetByUsernameAsync(username.Trim());
            if (user is null)
                return null;

            var isValid = _passwordHasher.VerifyPassword(password, user.Salt, user.PasswordHash);
            if (!isValid)
                return null;

            _logger.Info($"User logged in: {user.Username}");

            return MapToDto(user);
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

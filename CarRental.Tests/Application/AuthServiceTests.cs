using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;
using CarRental.Application.Exceptions;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Services;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.ValueObjects;
using Moq;
using Xunit;

namespace CarRental.Tests.Application
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _loggerMock = new Mock<ILogger>();

            _sut = new AuthService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_WithInvalidInput_ThrowsValidationException()
        {
            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.RegisterAsync("", "", "", UserRole.Employee));
        }

        [Fact]
        public async Task RegisterAsync_WhenUsernameAlreadyTaken_ThrowsValidationException()
        {
            _userRepositoryMock
                .Setup(r => r.GetByUsernameAsync("john"))
                .ReturnsAsync(new User("john", Email.Create("j@x.com"), "hash", "salt", UserRole.Employee));

            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.RegisterAsync("john", "john@example.com", "Password123", UserRole.Employee));
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_CreatesUserAndLogs()
        {
            _userRepositoryMock
                .Setup(r => r.GetByUsernameAsync("john"))
                .ReturnsAsync((User?)null);

            string saltOut = "salt";
            _passwordHasherMock
                .Setup(h => h.HashPassword("Password123", out saltOut))
                .Returns("HASH");

            User? addedUser = null;

            _userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => addedUser = u)
                .Returns(Task.CompletedTask);

            var dto = await _sut.RegisterAsync("john", "john@example.com", "Password123", UserRole.Employee);

            Assert.NotNull(addedUser);
            Assert.Equal("john", addedUser!.Username);
            Assert.Equal("john@example.com", addedUser.Email.Value);
            Assert.Equal("HASH", addedUser.PasswordHash);
            Assert.Equal("salt", addedUser.Salt);

            Assert.Equal(addedUser.Id, dto.Id);
            Assert.Equal(addedUser.Username, dto.Username);
            Assert.Equal(addedUser.Email.Value, dto.Email);

            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("User registered"))),
                Times.Once);
        }

        [Theory]
        [InlineData(null, "password")]
        [InlineData("john", null)]
        [InlineData("", "password")]
        [InlineData("john", "")]
        public async Task LoginAsync_WithMissingCredentials_ReturnsNull(string? username, string? password)
        {
            var result = await _sut.LoginAsync(username!, password!);

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            _userRepositoryMock
                .Setup(r => r.GetByUsernameAsync("john"))
                .ReturnsAsync((User?)null);

            var result = await _sut.LoginAsync("john", "Password123");

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordInvalid_ReturnsNull()
        {
            var user = new User("john", Email.Create("j@x.com"), "HASH", "salt", UserRole.Employee);

            _userRepositoryMock
                .Setup(r => r.GetByUsernameAsync("john"))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyPassword("Password123", "salt", "HASH"))
                .Returns(false);

            var result = await _sut.LoginAsync("john", "Password123");

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsDtoAndLogs()
        {
            var user = new User(1, "john", Email.Create("j@x.com"), "HASH", "salt", UserRole.Employee);

            _userRepositoryMock
                .Setup(r => r.GetByUsernameAsync("john"))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyPassword("Password123", "salt", "HASH"))
                .Returns(true);

            var result = await _sut.LoginAsync("john", "Password123");

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
            Assert.Equal("john", result.Username);

            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("User logged in"))),
                Times.Once);
        }
    }
}

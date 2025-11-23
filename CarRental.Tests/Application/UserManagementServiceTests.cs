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
    public class UserManagementServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly UserManagementService _sut;

        public UserManagementServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _loggerMock = new Mock<ILogger>();

            _sut = new UserManagementService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            _userRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<User>
                {
                    new User(1, "john", Email.Create("j@x.com"), "hash", "salt", UserRole.Employee)
                });

            var result = await _sut.GetAllAsync();

            var dto = Assert.Single(result);
            Assert.Equal(1, dto.Id);
            Assert.Equal("john", dto.Username);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ReturnsDto()
        {
            var user = new User(1, "john", Email.Create("j@x.com"), "hash", "salt", UserRole.Employee);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            var result = await _sut.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((User?)null);

            var result = await _sut.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidInput_ThrowsValidationException()
        {
            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.CreateAsync("", "", "", UserRole.Employee));
        }

        [Fact]
        public async Task CreateAsync_WhenUsernameTaken_ThrowsValidationException()
        {
            _userRepositoryMock
                .Setup(r => r.GetByUsernameAsync("john"))
                .ReturnsAsync(new User("john", Email.Create("j@x.com"), "hash", "salt", UserRole.Employee));

            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.CreateAsync("john", "john@example.com", "Pass123", UserRole.Employee));
        }

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesUserAndLogs()
        {
            _userRepositoryMock
                .Setup(r => r.GetByUsernameAsync("john"))
                .ReturnsAsync((User?)null);

            string saltOut = "salt";
            _passwordHasherMock
                .Setup(h => h.HashPassword("Pass123", out saltOut))
                .Returns("HASH");

            User? addedUser = null;

            _userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => addedUser = u)
                .Returns(Task.CompletedTask);

            var dto = await _sut.CreateAsync("john", "john@example.com", "Pass123", UserRole.Employee);

            Assert.NotNull(addedUser);
            Assert.Equal("john", addedUser!.Username);
            Assert.Equal("john@example.com", addedUser.Email.Value);
            Assert.Equal("HASH", addedUser.PasswordHash);
            Assert.Equal("salt", addedUser.Salt);

            Assert.Equal(addedUser.Id, dto.Id);

            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("User created by admin"))),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenUserNotFound_ThrowsNotFoundException()
        {
            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.UpdateAsync(1, "newName", "new@example.com"));
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidValues_ThrowsValidationException()
        {
            var user = new User(1, "john", Email.Create("j@x.com"), "hash", "salt", UserRole.Employee);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.UpdateAsync(1, "   ", "   "));
        }

        [Fact]
        public async Task UpdateAsync_WithValidUsernameAndEmail_UpdatesUserAndLogs()
        {
            var user = new User(1, "john", Email.Create("j@x.com"), "hash", "salt", UserRole.Employee);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            var dto = await _sut.UpdateAsync(1, " newName ", " new@example.com ");

            Assert.Equal("newName", user.Username);
            Assert.Equal("new@example.com", user.Email.Value);

            _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);

            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("User updated by admin"))),
                Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_WithEmptyPassword_ThrowsValidationException()
        {
            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.ChangePasswordAsync(1, ""));
        }

        [Fact]
        public async Task ChangePasswordAsync_WhenUserNotFound_ThrowsNotFoundException()
        {
            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.ChangePasswordAsync(1, "NewPass"));
        }

        [Fact]
        public async Task ChangePasswordAsync_WithValidData_UpdatesPasswordAndLogs()
        {
            var user = new User(1, "john", Email.Create("j@x.com"), "hash", "salt", UserRole.Employee);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            string saltOut = "newSalt";
            _passwordHasherMock
                .Setup(h => h.HashPassword("NewPass", out saltOut))
                .Returns("NEW_HASH");

            await _sut.ChangePasswordAsync(1, "NewPass");

            Assert.Equal("NEW_HASH", user.PasswordHash);
            Assert.Equal("newSalt", user.Salt);

            _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);

            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("User password changed by admin"))),
                Times.Once);
        }

        [Fact]
        public async Task ChangeRoleAsync_ChangesRoleAndLogs()
        {
            var user = new User(1, "john", Email.Create("j@x.com"), "hash", "salt", UserRole.Employee);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            await _sut.ChangeRoleAsync(1, UserRole.Admin);

            Assert.Equal(UserRole.Admin, user.Role);

            _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);

            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("User role changed by admin"))),
                Times.Once);
        }

        [Fact]
        public async Task PromoteToAdminAsync_SetsAdminRole()
        {
            var user = new User(1, "john", Email.Create("j@x.com"), "hash", "salt", UserRole.Employee);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            await _sut.PromoteToAdminAsync(1);

            Assert.Equal(UserRole.Admin, user.Role);
        }

        [Fact]
        public async Task DeleteAsync_WhenUserNotFound_ThrowsNotFoundException()
        {
            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.DeleteAsync(1));
        }

        [Fact]
        public async Task DeleteAsync_WhenUserExists_DeletesAndLogs()
        {
            var user = new User(1, "john", Email.Create("j@x.com"), "hash", "salt", UserRole.Employee);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            await _sut.DeleteAsync(1);

            _userRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);

            _loggerMock.Verify(
                l => l.Info(It.Is<string>(s => s.Contains("User deleted by admin"))),
                Times.Once);
        }
    }
}

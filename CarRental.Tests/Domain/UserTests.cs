using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Exceptions;
using CarRental.Domain.ValueObjects;
using Xunit;

namespace CarRental.Tests.Domain
{
    public class UserTests
    {
        [Fact]
        public void Constructor_WithValidData_SetsProperties()
        {
            var email = Email.Create("user@example.com");

            var user = new User(
                id: 1,
                username: "john",
                email: email,
                passwordHash: "hash",
                salt: "salt",
                role: UserRole.Employee);

            Assert.Equal(1, user.Id);
            Assert.Equal("john", user.Username);
            Assert.Equal(email, user.Email);
            Assert.Equal("hash", user.PasswordHash);
            Assert.Equal("salt", user.Salt);
            Assert.Equal(UserRole.Employee, user.Role);
        }

        [Fact]
        public void Constructor_WithEmptyUsername_ThrowsDomainException()
        {
            var email = Email.Create("user@example.com");

            Assert.Throws<DomainException>(() => new User("", email, "hash", "salt", UserRole.Employee));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateUsername_WithInvalidValue_ThrowsDomainException(string? newUsername)
        {
            var email = Email.Create("user@example.com");
            var user = new User("john", email, "hash", "salt", UserRole.Employee);

            Assert.Throws<DomainException>(() => user.UpdateUsername(newUsername!));
        }

        [Fact]
        public void UpdateEmail_WithNull_ThrowsDomainException()
        {
            var email = Email.Create("user@example.com");
            var user = new User("john", email, "hash", "salt", UserRole.Employee);

            Assert.Throws<DomainException>(() => user.UpdateEmail(null!));
        }

        [Theory]
        [InlineData(null, "salt")]
        [InlineData("", "salt")]
        [InlineData("hash", null)]
        [InlineData("hash", "")]
        public void UpdatePassword_WithInvalidValues_ThrowsDomainException(string? passwordHash, string? salt)
        {
            var email = Email.Create("user@example.com");
            var user = new User("john", email, "hash", "salt", UserRole.Employee);

            Assert.Throws<DomainException>(() => user.UpdatePassword(passwordHash!, salt!));
        }

        [Fact]
        public void ChangeRole_UpdatesRole()
        {
            var email = Email.Create("user@example.com");
            var user = new User("john", email, "hash", "salt", UserRole.Employee);

            user.ChangeRole(UserRole.Admin);

            Assert.Equal(UserRole.Admin, user.Role);
        }
    }
}

using CarRental.Infrastructure.Security;
using Xunit;

namespace CarRental.Tests.Infrastructure
{
    public class PasswordHasherTests
    {
        [Fact]
        public void HashPassword_WithSamePassword_ProducesDifferentSaltAndHash()
        {
            var hasher = new PasswordHasher();

            var hash1 = hasher.HashPassword("MyPassword123!", out var salt1);
            var hash2 = hasher.HashPassword("MyPassword123!", out var salt2);

            Assert.NotEqual(salt1, salt2);
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            var hasher = new PasswordHasher();

            var hash = hasher.HashPassword("Secret123!", out var salt);

            var result = hasher.VerifyPassword("Secret123!", salt, hash);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithWrongPassword_ReturnsFalse()
        {
            var hasher = new PasswordHasher();

            var hash = hasher.HashPassword("Secret123!", out var salt);

            var result = hasher.VerifyPassword("WrongPassword", salt, hash);

            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithInvalidBase64_ReturnsFalse()
        {
            var hasher = new PasswordHasher();

            var result = hasher.VerifyPassword("Secret123!", "not-base64", "also-not-base64");

            Assert.False(result);
        }
    }
}

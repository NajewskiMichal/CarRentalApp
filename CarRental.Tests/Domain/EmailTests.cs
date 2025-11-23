using System;
using CarRental.Domain.ValueObjects;
using Xunit;

namespace CarRental.Tests.Domain
{
    public class EmailTests
    {
        [Theory]
        [InlineData("alice@example.com")]
        [InlineData("bob.smith+tag@sub.domain.org")]
        public void Create_WithValidEmail_ReturnsValueObject(string email)
        {
            var valueObject = Email.Create(email);

            Assert.Equal(email.Trim(), valueObject.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("bad")]
        [InlineData("a@b")]
        public void Create_WithInvalidEmail_Throws(string email)
        {
            Assert.Throws<ArgumentException>(() => Email.Create(email));
        }

        [Fact]
        public void Equality_IsCaseInsensitive()
        {
            var first = Email.Create("USER@example.com");
            var second = Email.Create("user@example.com");

            Assert.True(first == second);
            Assert.True(first.Equals(second));
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }
    }
}

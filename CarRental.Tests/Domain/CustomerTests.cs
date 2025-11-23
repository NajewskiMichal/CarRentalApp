using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Domain.ValueObjects;
using Xunit;

namespace CarRental.Tests.Domain
{
    public class CustomerTests
    {
        [Fact]
        public void Constructor_WithValidData_SetsPropertiesAndTrimsName()
        {
            var email = Email.Create("customer@example.com");

            var customer = new Customer(1, "  John Smith ", email);

            Assert.Equal(1, customer.Id);
            Assert.Equal("John Smith", customer.Name);
            Assert.Equal(email, customer.Email);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithEmptyName_ThrowsDomainException(string name)
        {
            var email = Email.Create("customer@example.com");

            Assert.Throws<DomainException>(() => new Customer(1, name, email));
        }

        [Fact]
        public void Constructor_WithoutId_SetsDefaultId()
        {
            var email = Email.Create("customer@example.com");

            var customer = new Customer("John Smith", email);

            Assert.Equal(0, customer.Id);
            Assert.Equal("John Smith", customer.Name);
            Assert.Equal(email, customer.Email);
        }
    }
}

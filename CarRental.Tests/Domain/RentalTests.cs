using System;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using Xunit;

namespace CarRental.Tests.Domain
{
    public class RentalTests
    {
        [Fact]
        public void Constructor_WithValidData_SetsPropertiesAndIsActive()
        {
            var rentDate = new DateTime(2024, 1, 10);

            var rental = new Rental(1, 10, 20, rentDate);

            Assert.Equal(1, rental.Id);
            Assert.Equal(10, rental.CustomerId);
            Assert.Equal(20, rental.CarId);
            Assert.Equal(rentDate, rental.RentDate);
            Assert.True(rental.IsActive);
            Assert.Null(rental.ReturnDate);
        }

        [Fact]
        public void Constructor_WithDefaultRentDate_ThrowsDomainException()
        {
            Assert.Throws<DomainException>(() => new Rental(1, 1, default));
        }

        [Fact]
        public void Return_BeforeRent_Throws()
        {
            var rental = new Rental(1, 1, 1, new DateTime(2024, 1, 10));

            Assert.Throws<DomainException>(() => rental.Return(new DateTime(2024, 1, 9)));
        }

        [Fact]
        public void Return_Twice_Throws()
        {
            var today = DateTime.Today;
            var rental = new Rental(1, 1, 1, today);

            rental.Return(today.AddDays(1));

            Assert.Throws<DomainException>(() => rental.Return(today.AddDays(2)));
        }

        [Fact]
        public void Return_WithValidDate_SetsReturnDateAndDeactivatesRental()
        {
            var rental = new Rental(1, 1, 1, new DateTime(2024, 1, 10));

            rental.Return(new DateTime(2024, 1, 15));

            Assert.False(rental.IsActive);
            Assert.Equal(new DateTime(2024, 1, 15), rental.ReturnDate);
        }
    }
}

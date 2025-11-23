using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using Xunit;

namespace CarRental.Tests.Domain
{
    public class CarTests
    {
        [Fact]
        public void Constructor_WithValidData_SetsPropertiesAndTrimsValues()
        {
            // arrange & act
            var car = new Car(1, "  Toyota  ", "  Corolla ", 2020, " VIN123 ");

            // assert
            Assert.Equal(1, car.Id);
            Assert.Equal("Toyota", car.Brand);
            Assert.Equal("Corolla", car.Model);
            Assert.Equal(2020, car.Year);
            Assert.Equal("VIN123", car.Vin);
        }

        [Fact]
        public void Constructor_WithoutId_SetsDefaultIdAndProperties()
        {
            var car = new Car("Toyota", "Corolla", 2020, "VIN123");

            Assert.Equal(0, car.Id); // Id is set later by persistence layer
            Assert.Equal("Toyota", car.Brand);
            Assert.Equal("Corolla", car.Model);
            Assert.Equal(2020, car.Year);
            Assert.Equal("VIN123", car.Vin);
        }

        [Theory]
        [InlineData("", "Model", 2020, "VIN")]
        [InlineData("Brand", "", 2020, "VIN")]
        [InlineData("Brand", "Model", 1500, "VIN")] // too old
        [InlineData("Brand", "Model", 2200, "VIN")] // too far in the future
        [InlineData("Brand", "Model", 2020, "")]
        public void Constructor_WithInvalidArguments_ThrowsDomainException(
            string brand,
            string model,
            int year,
            string vin)
        {
            Assert.Throws<DomainException>(() => new Car(1, brand, model, year, vin));
        }
    }
}

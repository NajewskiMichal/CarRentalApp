using CarRental.Domain.Exceptions;

namespace CarRental.Domain.Entities
{
    /// <summary>
    /// Represents a car that can be rented.
    /// </summary>
    public class Car : Entity
    {
        public string Brand { get; private set; }
        public string Model { get; private set; }
        public int Year { get; private set; }
        public string Vin { get; private set; }

        public Car(int id, string brand, string model, int year, string vin)
            : base(id)
        {
            SetBrand(brand);
            SetModel(model);
            SetYear(year);
            SetVin(vin);
        }

        public Car(string brand, string model, int year, string vin)
        {
            SetBrand(brand);
            SetModel(model);
            SetYear(year);
            SetVin(vin);
        }

        private void SetBrand(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
                throw new DomainException("Brand cannot be empty.");

            Brand = brand.Trim();
        }

        private void SetModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new DomainException("Model cannot be empty.");

            Model = model.Trim();
        }

        private void SetYear(int year)
        {
            // Simple range validation – can be adjusted if needed.
            if (year < 1950 || year > 2100)
                throw new DomainException("Year is out of valid range.");

            Year = year;
        }

        private void SetVin(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin))
                throw new DomainException("VIN cannot be empty.");

            Vin = vin.Trim();
        }
    }
}

using System;
using CarRental.Domain.Exceptions;

namespace CarRental.Domain.Entities
{
    /// <summary>
    /// Represents a rental of a car by a customer.
    /// </summary>
    public class Rental : Entity
    {
        public int CustomerId { get; private set; }
        public int CarId { get; private set; }
        public DateTime RentDate { get; private set; }
        public DateTime? ReturnDate { get; private set; }

        public bool IsActive => ReturnDate == null;

        public Rental(int id, int customerId, int carId, DateTime rentDate, DateTime? returnDate = null)
            : base(id)
        {
            if (rentDate == default)
                throw new DomainException("Rent date is required.");

            CustomerId = customerId;
            CarId = carId;
            RentDate = rentDate;
            ReturnDate = returnDate;
        }

        public Rental(int customerId, int carId, DateTime rentDate)
        {
            if (rentDate == default)
                throw new DomainException("Rent date is required.");

            CustomerId = customerId;
            CarId = carId;
            RentDate = rentDate;
        }

        public void Return(DateTime returnDate)
        {
            if (ReturnDate != null)
                throw new DomainException("Rental is already returned.");

            if (returnDate < RentDate)
                throw new DomainException("Return date cannot be earlier than rent date.");

            ReturnDate = returnDate;
        }
    }
}

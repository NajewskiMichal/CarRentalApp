using System;

namespace CarRental.Application.DTOs
{
    public class RentalDto
    {
        public int Id { get; init; }
        public int CustomerId { get; init; }
        public int CarId { get; init; }
        public DateTime RentDate { get; init; }
        public DateTime? ReturnDate { get; init; }
        public bool IsActive { get; init; }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Application.Interfaces.Services
{
    public interface IRentalService
    {
        Task<RentalDto> RentCarAsync(int customerId, int carId, DateTime rentDate);
        Task ReturnCarAsync(int rentalId, DateTime returnDate);
        Task<IReadOnlyList<RentalDto>> GetAllAsync();
        Task<IReadOnlyList<RentalDto>> GetActiveAsync();
    }
}

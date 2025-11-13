using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Application.Interfaces.Services
{
    public interface ICarService
    {
        Task<IReadOnlyList<CarDto>> GetAllAsync();
        Task<CarDto> GetByIdAsync(int id);
        Task<CarDto> AddAsync(string brand, string model, int year, string vin);
        Task UpdateAsync(int id, string brand, string model, int year, string vin);
        Task DeleteAsync(int id);
        Task<IReadOnlyList<CarDto>> SearchByBrandAsync(string brand);
    }
}

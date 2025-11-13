using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<IReadOnlyList<CustomerDto>> GetAllAsync();
        Task<CustomerDto> GetByIdAsync(int id);
        Task<CustomerDto> AddAsync(string name, string email);
        Task UpdateAsync(int id, string name, string email);
        Task DeleteAsync(int id);
        Task<IReadOnlyList<CustomerDto>> SearchByNameAsync(string name);
    }
}

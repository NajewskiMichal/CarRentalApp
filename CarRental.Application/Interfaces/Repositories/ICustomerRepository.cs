using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int id);
        Task<IReadOnlyList<Customer>> GetAllAsync();
        Task AddAsync(Customer entity);
        Task UpdateAsync(Customer entity);
        Task DeleteAsync(int id);

        Task<IReadOnlyList<Customer>> SearchByNameAsync(string name);

        
        Task<IReadOnlyList<Customer>> GetAllIncludingInactiveAsync();

        
        Task<IReadOnlyList<Customer>> GetInactiveAsync();

        
        Task SetActiveAsync(int id, bool isActive);
    }
}

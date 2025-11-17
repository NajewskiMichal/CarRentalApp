using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories
{
    public interface ICarRepository
    {
        Task<Car?> GetByIdAsync(int id);
        Task<IReadOnlyList<Car>> GetAllAsync();
        Task AddAsync(Car entity);
        Task UpdateAsync(Car entity);
        Task DeleteAsync(int id);

        Task<IReadOnlyList<Car>> SearchByBrandAsync(string brand);

        
        Task<IReadOnlyList<Car>> GetAllIncludingInactiveAsync();

        
        Task<IReadOnlyList<Car>> GetInactiveAsync();

        
        Task SetActiveAsync(int id, bool isActive);
    }
}

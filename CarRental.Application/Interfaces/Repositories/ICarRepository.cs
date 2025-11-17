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

        // --- NOWE METODY DLA ADMINA ---

        /// <summary>
        /// Zwraca wszystkie auta, zarówno aktywne, jak i archiwalne.
        /// </summary>
        Task<IReadOnlyList<Car>> GetAllIncludingInactiveAsync();

        /// <summary>
        /// Zwraca tylko auta archiwalne (IsActive = 0).
        /// </summary>
        Task<IReadOnlyList<Car>> GetInactiveAsync();

        /// <summary>
        /// Ustawia flagę IsActive dla danego auta (true = aktywne, false = archiwalne).
        /// </summary>
        Task SetActiveAsync(int id, bool isActive);
    }
}

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

        // --- NOWE METODY DLA ADMINA ---

        /// <summary>
        /// Zwraca wszystkich klientów, zarówno aktywnych, jak i archiwalnych.
        /// </summary>
        Task<IReadOnlyList<Customer>> GetAllIncludingInactiveAsync();

        /// <summary>
        /// Zwraca tylko klientów archiwalnych (IsActive = 0).
        /// </summary>
        Task<IReadOnlyList<Customer>> GetInactiveAsync();

        /// <summary>
        /// Ustawia flagę IsActive dla danego klienta (true = aktywny, false = archiwalny).
        /// </summary>
        Task SetActiveAsync(int id, bool isActive);
    }
}

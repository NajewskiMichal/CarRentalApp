using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<IReadOnlyList<User>> GetAllAsync();
        Task AddAsync(User entity);
        Task UpdateAsync(User entity);
        Task DeleteAsync(int id);

        Task<User?> GetByUsernameAsync(string username);

        // --- NOWE METODY DLA ADMINA ---

        /// <summary>
        /// Zwraca wszystkich użytkowników, zarówno aktywnych, jak i archiwalnych.
        /// </summary>
        Task<IReadOnlyList<User>> GetAllIncludingInactiveAsync();

        /// <summary>
        /// Zwraca tylko użytkowników archiwalnych (IsActive = 0).
        /// </summary>
        Task<IReadOnlyList<User>> GetInactiveAsync();

        /// <summary>
        /// Ustawia flagę IsActive dla danego użytkownika (true = aktywny, false = archiwalny).
        /// </summary>
        Task SetActiveAsync(int id, bool isActive);
    }
}

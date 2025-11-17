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

        
        Task<IReadOnlyList<User>> GetAllIncludingInactiveAsync();

       
        Task<IReadOnlyList<User>> GetInactiveAsync();

       
        Task SetActiveAsync(int id, bool isActive);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;
using CarRental.Domain.Enums;

namespace CarRental.Application.Interfaces.Services
{
    public interface IUserManagementService
    {
        Task<IReadOnlyList<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);

        Task<UserDto> CreateAsync(string username, string email, string password, UserRole role);
        Task<UserDto> UpdateAsync(int id, string? username, string? email);
        Task ChangePasswordAsync(int id, string newPassword);
        Task ChangeRoleAsync(int id, UserRole newRole);
        Task PromoteToAdminAsync(int id);
        Task DeleteAsync(int id);
    }
}

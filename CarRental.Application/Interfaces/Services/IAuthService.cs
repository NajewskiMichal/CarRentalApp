using System.Threading.Tasks;
using CarRental.Application.DTOs;
using CarRental.Domain.Enums;

namespace CarRental.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(string username, string email, string password, UserRole role);
        Task<UserDto?> LoginAsync(string username, string password);
    }
}

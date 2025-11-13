using CarRental.Domain.Enums;

namespace CarRental.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public UserRole Role { get; init; }
    }
}

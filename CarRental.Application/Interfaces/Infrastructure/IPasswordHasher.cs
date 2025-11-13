namespace CarRental.Application.Interfaces.Infrastructure
{
    /// <summary>
    /// Abstraction for password hashing and verification.
    /// Implemented in the Infrastructure layer.
    /// </summary>
    public interface IPasswordHasher
    {
        string HashPassword(string password, out string salt);
        bool VerifyPassword(string password, string salt, string passwordHash);
    }
}

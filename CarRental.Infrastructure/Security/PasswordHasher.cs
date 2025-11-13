using System;
using System.Security.Cryptography;
using System.Text;
using CarRental.Application.Interfaces.Infrastructure;

namespace CarRental.Infrastructure.Security
{
    /// <summary>
    /// Password hasher using PBKDF2 (Rfc2898DeriveBytes).
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;        // 128-bit
        private const int KeySize = 32;         // 256-bit
        private const int Iterations = 100_000; // quite safe for local app

        public string HashPassword(string password, out string salt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            using var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[SaltSize];
            rng.GetBytes(saltBytes);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);

            salt = Convert.ToBase64String(saltBytes);
            var hash = Convert.ToBase64String(key);

            return hash;
        }

        public bool VerifyPassword(string password, string salt, string passwordHash)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (salt == null) throw new ArgumentNullException(nameof(salt));
            if (passwordHash == null) throw new ArgumentNullException(nameof(passwordHash));

            var saltBytes = Convert.FromBase64String(salt);
            var hashBytes = Convert.FromBase64String(passwordHash);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);
            var computedHash = pbkdf2.GetBytes(KeySize);

            return CryptographicOperations.FixedTimeEquals(hashBytes, computedHash);
        }
    }
}

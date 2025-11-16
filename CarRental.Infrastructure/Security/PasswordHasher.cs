using System;
using System.Security.Cryptography;
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

        /// <summary>
        /// Hashes a password and returns Base64-encoded hash + salt (out).
        /// </summary>
        public string HashPassword(string password, out string salt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            // generate random salt
            var saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // derive key
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA256);

            var hashBytes = pbkdf2.GetBytes(KeySize);

            salt = Convert.ToBase64String(saltBytes);
            var passwordHash = Convert.ToBase64String(hashBytes);

            return passwordHash;
        }

        /// <summary>
        /// Verifies a password against Base64-encoded salt and hash.
        /// Gdy salt/hash nie są poprawnym Base64 (np. RESET_ME) – zwraca false, zamiast rzucać wyjątek.
        /// </summary>
        public bool VerifyPassword(string password, string salt, string passwordHash)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (salt == null) throw new ArgumentNullException(nameof(salt));
            if (passwordHash == null) throw new ArgumentNullException(nameof(passwordHash));

            byte[] saltBytes;
            byte[] hashBytes;

            try
            {
                saltBytes = Convert.FromBase64String(salt);
                hashBytes = Convert.FromBase64String(passwordHash);
            }
            catch (FormatException)
            {
                
                return false;
            }

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA256);

            var computedHash = pbkdf2.GetBytes(KeySize);

            return CryptographicOperations.FixedTimeEquals(hashBytes, computedHash);
        }
    }
}

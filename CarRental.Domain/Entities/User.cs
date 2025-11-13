using CarRental.Domain.Enums;
using CarRental.Domain.Exceptions;
using CarRental.Domain.ValueObjects;

namespace CarRental.Domain.Entities
{
    /// <summary>
    /// Represents a system user (for authentication & authorization).
    /// </summary>
    public class User : Entity
    {
        public string Username { get; private set; }
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Salt { get; private set; }
        public UserRole Role { get; private set; }

        public User(
            int id,
            string username,
            Email email,
            string passwordHash,
            string salt,
            UserRole role)
            : base(id)
        {
            SetUsername(username);
            Email = email;
            SetPassword(passwordHash, salt);
            Role = role;
        }

        public User(
            string username,
            Email email,
            string passwordHash,
            string salt,
            UserRole role)
        {
            SetUsername(username);
            Email = email;
            SetPassword(passwordHash, salt);
            Role = role;
        }

        private void SetUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new DomainException("Username cannot be empty.");

            Username = username.Trim();
        }

        private void SetPassword(string passwordHash, string salt)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password hash cannot be empty.");

            if (string.IsNullOrWhiteSpace(salt))
                throw new DomainException("Salt cannot be empty.");

            PasswordHash = passwordHash;
            Salt = salt;
        }
    }
}

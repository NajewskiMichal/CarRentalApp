using CarRental.Domain.Exceptions;
using CarRental.Domain.ValueObjects;

namespace CarRental.Domain.Entities
{
    /// <summary>
    /// Represents a customer who can rent cars.
    /// </summary>
    public class Customer : Entity
    {
        public string Name { get; private set; }
        public Email Email { get; private set; }

        public Customer(int id, string name, Email email)
            : base(id)
        {
            SetName(name);
            Email = email;
        }

        public Customer(string name, Email email)
        {
            SetName(name);
            Email = email;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name cannot be empty.");

            Name = name.Trim();
        }
    }
}

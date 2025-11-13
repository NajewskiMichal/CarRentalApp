using System;
using System.Text.RegularExpressions;

namespace CarRental.Domain.ValueObjects
{
    /// <summary>
    /// Immutable value object representing an email address.
    /// Always created through the factory method to ensure validation.
    /// </summary>
    public sealed class Email : IEquatable<Email>
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty.", nameof(value));

            value = value.Trim();

            if (!EmailRegex.IsMatch(value))
                throw new ArgumentException("Invalid email format.", nameof(value));

            return new Email(value);
        }

        public override string ToString() => Value;

        #region Equality

        public bool Equals(Email? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj) => Equals(obj as Email);

        public override int GetHashCode() =>
            StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

        public static bool operator ==(Email? left, Email? right) =>
            Equals(left, right);

        public static bool operator !=(Email? left, Email? right) =>
            !Equals(left, right);

        #endregion
    }
}

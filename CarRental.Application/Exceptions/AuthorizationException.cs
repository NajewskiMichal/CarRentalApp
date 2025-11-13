namespace CarRental.Application.Exceptions
{
    /// <summary>
    /// Thrown when a user attempts to perform an action without sufficient permissions.
    /// </summary>
    public class AuthorizationException : ApplicationExceptionBase
    {
        public AuthorizationException(string message)
            : base(message)
        {
        }
    }
}

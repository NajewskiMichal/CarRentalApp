using System;

namespace CarRental.Application.Exceptions
{
    /// <summary>
    /// Base type for application-level exceptions (use-case / service layer).
    /// </summary>
    public abstract class ApplicationExceptionBase : Exception
    {
        protected ApplicationExceptionBase(string message)
            : base(message)
        {
        }

        protected ApplicationExceptionBase(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

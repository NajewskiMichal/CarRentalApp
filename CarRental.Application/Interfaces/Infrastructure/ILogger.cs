using System;

namespace CarRental.Application.Interfaces.Infrastructure
{
    /// <summary>
    /// Simple logging abstraction for the application layer.
    /// </summary>
    public interface ILogger
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message, Exception? exception = null);
    }
}

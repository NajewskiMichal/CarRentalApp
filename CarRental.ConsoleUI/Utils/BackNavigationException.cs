using System;

namespace CarRental.ConsoleUI.Utils
{
    /// <summary>
    /// Exception used to signal "go back" navigation from nested screens.
    /// Thrown typically when user enters 'B'.
    /// </summary>
    public class BackNavigationException : Exception
    {
        public BackNavigationException()
            : base("Back navigation requested.")
        {
        }
    }
}

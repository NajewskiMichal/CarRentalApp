using System;

namespace CarRental.ConsoleUI.Utils
{
    /// <summary>
    /// Exception used to signal that the user requested logout.
    /// Caught in Program.Main to start a new login flow.
    /// </summary>
    public class LogoutRequestedException : Exception
    {
        public LogoutRequestedException()
            : base("Logout requested.")
        {
        }
    }
}

using CarRental.Application.DTOs;
using CarRental.Domain.Enums;

namespace CarRental.ConsoleUI.Authentication
{
    /// <summary>
    /// Holds information about the currently logged-in user for the console UI.
    /// </summary>
    public static class UserContext
    {
        public static UserDto? CurrentUser { get; private set; }

        public static void SetCurrentUser(UserDto? user)
        {
            CurrentUser = user;
        }

        public static bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
    }
}

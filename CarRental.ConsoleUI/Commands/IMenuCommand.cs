using System.Threading.Tasks;

namespace CarRental.ConsoleUI.Commands
{
    /// <summary>
    /// Represents a single menu command (Command pattern).
    /// </summary>
    public interface IMenuCommand
    {
        string Key { get; }
        string Description { get; }

        Task ExecuteAsync();
    }

    /// <summary>
    /// Marker interface for commands that should only be visible to admin users in the main menu.
    /// </summary>
    public interface IAdminMenuCommand : IMenuCommand
    {
    }
}

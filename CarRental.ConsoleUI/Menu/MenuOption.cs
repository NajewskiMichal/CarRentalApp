namespace CarRental.ConsoleUI.Menu
{
    public class MenuOption
    {
        public string Key { get; }
        public string Description { get; }

        public MenuOption(string key, string description)
        {
            Key = key;
            Description = description;
        }
    }
}

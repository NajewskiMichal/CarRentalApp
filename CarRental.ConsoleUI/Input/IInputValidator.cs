namespace CarRental.ConsoleUI.Input
{
    public interface IInputValidator
    {
        string ReadInput(string prompt);
        string ValidateInputNotEmpty(string prompt);
        int ValidateIntegerInput(string prompt);
        int ValidateIntegerInput(string prompt, int minValue);
    }
}

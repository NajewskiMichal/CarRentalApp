namespace CarRental.Application.Exceptions
{
    /// <summary>
    /// Thrown when a requested entity cannot be found.
    /// </summary>
    public class NotFoundException : ApplicationExceptionBase
    {
        public string EntityName { get; }
        public object? Key { get; }

        public NotFoundException(string entityName, object? key = null)
            : base(BuildMessage(entityName, key))
        {
            EntityName = entityName;
            Key = key;
        }

        private static string BuildMessage(string entityName, object? key)
        {
            if (key is null)
                return $"{entityName} was not found.";

            return $"{entityName} with key '{key}' was not found.";
        }
    }
}

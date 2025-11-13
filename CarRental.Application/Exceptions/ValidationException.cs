using System.Collections.Generic;
using System.Linq;

namespace CarRental.Application.Exceptions
{
    /// <summary>
    /// Thrown when input data does not pass validation rules.
    /// </summary>
    public class ValidationException : ApplicationExceptionBase
    {
        public IReadOnlyCollection<string> Errors { get; }

        public ValidationException(IEnumerable<string> errors)
            : base(BuildMessage(errors))
        {
            Errors = errors.ToArray();
        }

        private static string BuildMessage(IEnumerable<string> errors)
        {
            var list = errors.ToArray();
            if (list.Length == 0)
                return "Validation failed.";

            return "Validation failed: " + string.Join("; ", list);
        }
    }
}

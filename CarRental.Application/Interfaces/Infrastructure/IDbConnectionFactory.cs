using System.Data.Common;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces.Infrastructure
{
    /// <summary>
    /// Factory for creating database connections.
    /// Implementation will live in the Infrastructure project.
    /// </summary>
    public interface IDbConnectionFactory
    {
        Task<DbConnection> CreateOpenConnectionAsync();
    }
}

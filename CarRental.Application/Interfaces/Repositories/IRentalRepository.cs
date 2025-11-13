using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories
{
    public interface IRentalRepository : IRepository<Rental>
    {
        Task<IReadOnlyList<Rental>> GetActiveRentalsAsync();
    }
}

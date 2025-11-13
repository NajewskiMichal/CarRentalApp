using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories
{
    public interface ICarRepository : IRepository<Car>
    {
        Task<IReadOnlyList<Car>> SearchByBrandAsync(string brand);
    }
}

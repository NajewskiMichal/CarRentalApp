using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<IReadOnlyList<Customer>> SearchByNameAsync(string name);
    }
}

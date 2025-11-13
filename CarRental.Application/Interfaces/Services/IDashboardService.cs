using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
    }
}

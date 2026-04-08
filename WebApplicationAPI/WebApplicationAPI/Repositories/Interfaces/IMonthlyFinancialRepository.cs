using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.Repositories.Interfaces
{
    public interface IMonthlyFinancialRepository
    {
        Task<int> CreateAsync(MonthlyFinancialControl entity);

        Task<MonthlyFinancialControl> UpdateAsync(MonthlyFinancialControl entity);

        Task<bool> DeleteAsync(int id);

        Task<MonthlyFinancialControl> GetByIdAsync(int id);

        Task<List<MonthlyFinancialControl>> GetAllAsync();

        Task<MonthlyFinancialControl> GetByYearAndMonthAsync(int year, int month);

        Task<decimal> GetExpensesTotalByYearAndMonthAsync(int year, int month);
    }
}

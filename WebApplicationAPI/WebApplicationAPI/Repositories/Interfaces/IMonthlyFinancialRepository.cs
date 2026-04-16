using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.Repositories.Interfaces
{
    public interface IMonthlyFinancialRepository
    {
        Task<int> CreateAsync(MonthlyFinancialControl entity);

        Task<MonthlyFinancialControl> UpdateAsync(MonthlyFinancialControl entity);

        Task<bool> DeleteAsync(int id, int userId);

        Task<MonthlyFinancialControl> GetByIdAsync(int id, int userId);

        Task<List<MonthlyFinancialControl>> GetAllAsync(int userId);

        Task<List<MonthlyFinancialControl>> GetByUserIdAsync(int userId); // ? Novo método

        Task<MonthlyFinancialControl> GetByYearAndMonthAsync(int userId, int year, int month);

        Task<decimal> GetExpensesTotalByYearAndMonthAsync(int userId, int year, int month);
    }
}

using System.Collections.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationAPI.DTO;

namespace WebApplicationAPI.Services.Interfaces
{
    public interface IMonthlyFinancialService
    {
        Task<MonthlyFinancialResponse> CreateAsync(CreateMonthlyFinancialRequest request);

        Task<MonthlyFinancialResponse> UpdateAsync(int id, UpdateMonthlyFinancialRequest request);

        Task<bool> DeleteAsync(int id);

        Task<MonthlyFinancialResponse> GetByIdAsync(int id);

        Task<List<MonthlyFinancialResponse>> GetAllAsync();

        Task<MonthlyFinancialResponse> GetByYearAndMonthAsync(int year, int month);

        Task<List<MonthlyFinancialResponse>> GetByYearAsync(int year);
    }
}

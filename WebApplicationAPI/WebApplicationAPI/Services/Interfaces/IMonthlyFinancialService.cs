using System.Collections.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationAPI.DTO;

namespace WebApplicationAPI.Services.Interfaces
{
    public interface IMonthlyFinancialService
    {
        Task<MonthlyFinancialResponse> CreateAsync(int userId, CreateMonthlyFinancialRequest request); // ? Adicionado userId

        Task<MonthlyFinancialResponse> UpdateAsync(int id, int userId, UpdateMonthlyFinancialRequest request); // ? Adicionado userId

        Task<bool> DeleteAsync(int id, int userId); // ? Adicionado userId

        Task<MonthlyFinancialResponse> GetByIdAsync(int id, int userId); // ? Adicionado userId

        Task<List<MonthlyFinancialResponse>> GetAllAsync(int userId); // ? Adicionado userId

        Task<MonthlyFinancialResponse> GetByYearAndMonthAsync(int userId, int year, int month); // ? Adicionado userId

        Task<List<MonthlyFinancialResponse>> GetByYearAsync(int userId, int year); // ? Adicionado userId
    }
}

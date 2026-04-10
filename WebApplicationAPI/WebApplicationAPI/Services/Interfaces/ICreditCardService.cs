using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebApplicationAPI.DTO;

namespace WebApplicationAPI.Services.Interfaces
{
    public interface ICreditCardService
    {
        Task<CreditCardResponse> CreateAsync(CreateCreditCardRequest request);
        Task<CreditCardResponse> UpdateAsync(int id, UpdateCreditCardRequest request);
        Task<bool> DeleteAsync(int id);
        Task<CreditCardResponse> GetByIdAsync(int id);
        Task<List<CreditCardResponse>> GetAllAsync();
        Task<CreditCardExpenseResponse> CreateExpenseAsync(int creditCardId, CreateCreditCardExpenseRequest request);
        Task<CreditCardExpenseResponse> UpdateExpenseAsync(int expenseId, UpdateCreditCardExpenseRequest request);
        Task<bool> DeleteExpenseAsync(int expenseId);
        Task<CreditCardExpenseResponse> GetExpenseByIdAsync(int expenseId);
        Task<List<CreditCardExpenseResponse>> GetExpensesByCardAsync(int creditCardId, int? month = null, int? year = null, string? category = null);
        Task<CsvImportResult> ImportCsvAsync(int creditCardId, Stream fileStream);
        Task<CreditCardStatementResponse> GetStatementAsync(int creditCardId, int month, int year);
        Task<List<CategoryAnalysisResponse>> GetCategoryAnalysisAsync(int creditCardId, int month, int year);
        Task<CreditCardStatementResponse> GetStatementByPeriodAsync(int creditCardId, DateTime startDate, DateTime endDate);
    }
}

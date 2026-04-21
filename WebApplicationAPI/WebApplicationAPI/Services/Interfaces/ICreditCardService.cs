using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebApplicationAPI.DTO;

namespace WebApplicationAPI.Services.Interfaces
{
    public interface ICreditCardService
    {
        Task<CreditCardResponse> CreateAsync(int userId, CreateCreditCardRequest request);
        Task<CreditCardResponse> UpdateAsync(int id, int userId, UpdateCreditCardRequest request);
        Task<bool> DeleteAsync(int id, int userId);
        Task<CreditCardResponse> GetByIdAsync(int id, int userId);
        Task<List<CreditCardResponse>> GetAllAsync(int userId);
        Task<CreditCardExpenseResponse> CreateExpenseAsync(int creditCardId, int userId, CreateCreditCardExpenseRequest request);
        Task<CreditCardExpenseResponse> UpdateExpenseAsync(int expenseId, int userId, UpdateCreditCardExpenseRequest request);
        Task<bool> DeleteExpenseAsync(int expenseId, int userId);
        Task<CreditCardExpenseResponse> GetExpenseByIdAsync(int expenseId, int userId);
        Task<List<CreditCardExpenseResponse>> GetExpensesByCardAsync(int creditCardId, int userId, int? month = null, int? year = null, string? category = null);
        Task<CsvImportResult> ImportCsvAsync(int creditCardId, int userId, Stream fileStream);
        Task<CreditCardStatementResponse> GetStatementAsync(int creditCardId, int userId, int month, int year);
        Task<List<CategoryAnalysisResponse>> GetCategoryAnalysisAsync(int creditCardId, int userId, int month, int year);
        Task<CreditCardStatementResponse> GetStatementByPeriodAsync(int creditCardId, int userId, DateTime startDate, DateTime endDate);

        Task<SimulatePurchaseResponse> SimulatePurchaseAsync(int creditCardId, int userId, SimulatePurchaseRequest request);
        Task<InstallmentPurchaseResponse> ConfirmPurchaseAsync(int creditCardId, int userId, ConfirmPurchaseRequest request);
        Task<List<InstallmentPurchaseResponse>> GetInstallmentPurchasesAsync(int creditCardId, int userId);
    }
}

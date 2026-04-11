using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.Repositories.Interfaces
{
    public interface ICreditCardRepository
    {
        Task<int> CreateAsync(CreditCard creditCard);
        Task<CreditCard> UpdateAsync(CreditCard creditCard);
        Task<bool> DeleteAsync(int id);
        Task<CreditCard> GetByIdAsync(int id);
        Task<List<CreditCard>> GetAllAsync();

        Task<int> CreateExpenseAsync(CreditCardExpense expense);
        Task<CreditCardExpense> UpdateExpenseAsync(CreditCardExpense expense);
        Task<bool> DeleteExpenseAsync(int id);
        Task<CreditCardExpense> GetExpenseByIdAsync(int id);
        Task<List<CreditCardExpense>> GetExpensesByCardAsync(int creditCardId, int? month = null, int? year = null, string? category = null);

        Task<List<CreditCardExpense>> GetStatementAsync(int creditCardId, int month, int year);
        Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync(int creditCardId, int month, int year);

        Task<bool> ExpenseExistsAsync(int creditCardId, DateTime purchaseDate, string description, decimal amount);

        Task<List<CreditCardExpense>> GetExpensesByPeriodAsync(int creditCardId, DateTime startDate, DateTime endDate);

        Task<int> CreateInstallmentPurchaseAsync(InstallmentPurchase purchase);
        Task<List<InstallmentPurchase>> GetActiveInstallmentsByMonthAsync(int creditCardId, int month, int year);
        Task<List<InstallmentPurchase>> GetAllActiveInstallmentsByMonthAsync(int month, int year);
        Task<List<InstallmentPurchase>> GetAllInstallmentPurchasesAsync(int creditCardId);
        Task<bool> UpdateInstallmentPurchaseStatusAsync(int id, string status);
    }
}

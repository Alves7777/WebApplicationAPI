using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.Repositories.Interfaces
{
    public interface ICreditCardRepository
    {
        // Credit Card CRUD
        Task<int> CreateAsync(CreditCard creditCard);
        Task<CreditCard> UpdateAsync(CreditCard creditCard);
        Task<bool> DeleteAsync(int id);
        Task<CreditCard> GetByIdAsync(int id);
        Task<List<CreditCard>> GetAllAsync();

        // Credit Card Expense CRUD
        Task<int> CreateExpenseAsync(CreditCardExpense expense);
        Task<CreditCardExpense> UpdateExpenseAsync(CreditCardExpense expense);
        Task<bool> DeleteExpenseAsync(int id);
        Task<CreditCardExpense> GetExpenseByIdAsync(int id);
        Task<List<CreditCardExpense>> GetExpensesByCardAsync(int creditCardId, int? month = null, int? year = null, string? category = null);

        // Analytics
        Task<List<CreditCardExpense>> GetStatementAsync(int creditCardId, int month, int year);
        Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync(int creditCardId, int month, int year);

        // Verificar duplicidade
        Task<bool> ExpenseExistsAsync(int creditCardId, DateTime purchaseDate, string description, decimal amount);
    }
}

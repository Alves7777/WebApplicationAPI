using System.Collections.Generic;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.Services.Interfaces
{
    public interface IFinancialCalculatorService
    {
        decimal GetTotalExpenses(IEnumerable<Expense> expenses);
        Dictionary<string, decimal> GetTotalByCategory(IEnumerable<Expense> expenses);
        Dictionary<string, decimal> GetTotalByStatus(IEnumerable<Expense> expenses);
        decimal CalculateBalance(decimal salary, decimal totalExpenses);
        decimal CalculateAvailableToSpend(decimal balance, decimal reserve);
    }
}
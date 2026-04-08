using System.Collections.Generic;
using System.Linq;
using WebApplicationAPI.Models;
using WebApplicationAPI.Services.Interfaces;

namespace WebApplicationAPI.Services
{
    public class FinancialCalculatorService : IFinancialCalculatorService
    {
        public decimal GetTotalExpenses(IEnumerable<Expense> expenses)
        {
            return expenses.Sum(e => e.Amount);
        }

        public Dictionary<string, decimal> GetTotalByCategory(IEnumerable<Expense> expenses)
        {
            return expenses.GroupBy(e => e.Category)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
        }

        public Dictionary<string, decimal> GetTotalByStatus(IEnumerable<Expense> expenses)
        {
            return expenses.GroupBy(e => e.Status)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
        }

        public decimal CalculateBalance(decimal salary, decimal totalExpenses)
        {
            return salary - totalExpenses;
        }

        public decimal CalculateAvailableToSpend(decimal balance, decimal reserve)
        {
            return balance - reserve;
        }
    }
}
using System;
using WebApplicationAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplicationAPI.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Task<Expense> CreateExpenseAsync(Expense expense);
        Task<Expense> UpdateExpenseAsync(Expense expense);
        Task<bool> DeleteExpenseAsync(int id);
        Task<List<Expense>> GetExpensesAsync(int? month, int? year, string? category, string? status, string? paymentMethod);
        Task<Expense> GetExpenseByIdAsync(int id);
        Task<decimal> GetTotalExpensesByMonthAsync(int month, int year);
        Task<decimal> GetTotalExpensesByMonthExcludingCategoryAsync(int month, int year, string excludeCategory);
    }
}

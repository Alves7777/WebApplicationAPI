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
        Task<List<Expense>> GetExpensesByUserIdAsync(int userId, int? month = null, int? year = null, string? category = null, string? status = null, string? paymentMethod = null); // ? Novo
        Task<Expense> GetExpenseByIdAsync(int id, int userId);
        Task<decimal> GetTotalExpensesByMonthAsync(int month, int year);
        Task<decimal> GetTotalExpensesByMonthExcludingCategoryAsync(int month, int year, string excludeCategory);
    }
}

using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using WebApplicationAPI.Models;
using WebApplicationAPI.Repositories.Interfaces;

namespace WebApplicationAPI.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly string _connectionString;
        public ExpenseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Expense> CreateExpenseAsync(Expense expense)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", expense.UserId);
            parameters.Add("@Month", expense.Month);
            parameters.Add("@Year", expense.Year);
            parameters.Add("@Description", expense.Description);
            parameters.Add("@Amount", expense.Amount);
            parameters.Add("@Category", expense.Category);
            parameters.Add("@Status", expense.Status);
            parameters.Add("@PaymentMethod", expense.PaymentMethod);
            parameters.Add("@CreatedAt", expense.CreatedAt);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync("sp_CreateExpense", parameters, commandType: CommandType.StoredProcedure);
            expense.Id = parameters.Get<int>("@Id");
            return expense;
        }

        public async Task<Expense> UpdateExpenseAsync(Expense expense)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("sp_UpdateExpense", new
            {
                expense.Id,
                expense.Month,
                expense.UserId,
                expense.Year,
                expense.Description,
                expense.Amount,
                expense.Category,
                expense.Status,
                expense.PaymentMethod,
                expense.UpdatedBy
            }, commandType: CommandType.StoredProcedure);
            return expense;
        }

        public async Task<bool> DeleteExpenseAsync(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            var rows = await connection.ExecuteAsync("sp_DeleteExpense", new { Id = id, UserId = userId }, commandType: CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<List<Expense>> GetExpensesAsync(int? month, int? year, string category, string status, string paymentMethod)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<Expense>("sp_GetExpenses", new
            {
                Month = month,
                Year = year,
                Category = category,
                Status = status,
                PaymentMethod = paymentMethod
            }, commandType: CommandType.StoredProcedure);
            return result.AsList();
        }

        public async Task<List<Expense>> GetExpensesByUserIdAsync(int userId, int? month = null, int? year = null, string? category = null, string? status = null, string? paymentMethod = null)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<Expense>("sp_GetExpensesByUserId", new
            {
                UserId = userId,
                Month = month,
                Year = year,
                Category = category,
                Status = status,
                PaymentMethod = paymentMethod
            }, commandType: CommandType.StoredProcedure);
            return result.AsList();
        }

        public async Task<Expense> GetExpenseByIdAsync(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryFirstOrDefaultAsync<Expense>("sp_GetExpenseById", new { Id = id, UserId = userId }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<decimal> GetTotalExpensesByMonthAsync(int month, int year)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryFirstOrDefaultAsync<decimal>(
                "sp_GetExpensesTotalByYearAndMonth",
                new { Year = year, Month = month },
                commandType: CommandType.StoredProcedure
            );
            return result;
        }

        public async Task<decimal> GetTotalExpensesByMonthExcludingCategoryAsync(int month, int year, string excludeCategory)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryFirstOrDefaultAsync<decimal>(
                "sp_GetExpensesTotalByMonthExcludingCategory",
                new { Year = year, Month = month, ExcludeCategory = excludeCategory },
                commandType: CommandType.StoredProcedure
            );
            return result;
        }
    }
}

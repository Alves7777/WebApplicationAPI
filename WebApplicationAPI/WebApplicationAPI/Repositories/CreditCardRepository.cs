using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WebApplicationAPI.Models;
using WebApplicationAPI.Repositories.Interfaces;

namespace WebApplicationAPI.Repositories
{
    public class CreditCardRepository : ICreditCardRepository
    {
        private readonly string _connectionString;

        public CreditCardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ===== CREDIT CARD CRUD =====

        public async Task<int> CreateAsync(CreditCard creditCard)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@Name", creditCard.Name);
            parameters.Add("@Brand", creditCard.Brand);
            parameters.Add("@CardLimit", creditCard.CardLimit);
            parameters.Add("@ClosingDay", creditCard.ClosingDay);
            parameters.Add("@DueDay", creditCard.DueDay);
            parameters.Add("@IsActive", creditCard.IsActive);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_CreateCreditCard",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return parameters.Get<int>("@Id");
        }

        public async Task<CreditCard> UpdateAsync(CreditCard creditCard)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(
                "sp_UpdateCreditCard",
                new
                {
                    creditCard.Id,
                    creditCard.Name,
                    creditCard.Brand,
                    creditCard.CardLimit,
                    creditCard.ClosingDay,
                    creditCard.DueDay,
                    creditCard.IsActive
                },
                commandType: CommandType.StoredProcedure
            );

            return creditCard;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            var rows = await connection.ExecuteAsync(
                "sp_DeleteCreditCard",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

        public async Task<CreditCard> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<CreditCard>(
                "sp_GetCreditCardById",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<List<CreditCard>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryAsync<CreditCard>(
                "sp_GetAllCreditCards",
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        // ===== CREDIT CARD EXPENSE CRUD =====

        public async Task<int> CreateExpenseAsync(CreditCardExpense expense)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@CreditCardId", expense.CreditCardId);
            parameters.Add("@PurchaseDate", expense.PurchaseDate);
            parameters.Add("@Description", expense.Description);
            parameters.Add("@Amount", expense.Amount);
            parameters.Add("@Category", expense.Category);
            parameters.Add("@Month", expense.Month);
            parameters.Add("@Year", expense.Year);
            parameters.Add("@Status", expense.Status);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_CreateCreditCardExpense",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return parameters.Get<int>("@Id");
        }

        public async Task<CreditCardExpense> UpdateExpenseAsync(CreditCardExpense expense)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(
                "sp_UpdateCreditCardExpense",
                new
                {
                    expense.Id,
                    expense.PurchaseDate,
                    expense.Description,
                    expense.Amount,
                    expense.Category,
                    expense.Status
                },
                commandType: CommandType.StoredProcedure
            );

            return expense;
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            var rows = await connection.ExecuteAsync(
                "sp_DeleteCreditCardExpense",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

        public async Task<CreditCardExpense> GetExpenseByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<CreditCardExpense>(
                "sp_GetCreditCardExpenseById",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<List<CreditCardExpense>> GetExpensesByCardAsync(int creditCardId, int? month = null, int? year = null, string? category = null)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryAsync<CreditCardExpense>(
                "sp_GetCreditCardExpensesByCard",
                new
                {
                    CreditCardId = creditCardId,
                    Month = month,
                    Year = year,
                    Category = category
                },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        // ===== ANALYTICS =====

        public async Task<List<CreditCardExpense>> GetStatementAsync(int creditCardId, int month, int year)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryAsync<CreditCardExpense>(
                "sp_GetCreditCardStatement",
                new
                {
                    CreditCardId = creditCardId,
                    Month = month,
                    Year = year
                },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync(int creditCardId, int month, int year)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryAsync<(string Category, decimal TotalAmount)>(
                "sp_GetCreditCardExpensesByCategory",
                new
                {
                    CreditCardId = creditCardId,
                    Month = month,
                    Year = year
                },
                commandType: CommandType.StoredProcedure
            );

            return result.ToDictionary(x => x.Category ?? "Sem Categoria", x => x.TotalAmount);
        }
    }
}

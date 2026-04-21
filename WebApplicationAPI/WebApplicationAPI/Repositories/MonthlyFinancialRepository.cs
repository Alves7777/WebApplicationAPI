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
    public class MonthlyFinancialRepository : IMonthlyFinancialRepository
    {
        private readonly string _connectionString;

        public MonthlyFinancialRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateAsync(MonthlyFinancialControl entity)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", entity.UserId);
            parameters.Add("@Year", entity.Year);
            parameters.Add("@Month", entity.Month);
            parameters.Add("@Money", entity.Money);
            parameters.Add("@RV", entity.RV);
            parameters.Add("@Debit", entity.Debit);
            parameters.Add("@Others", entity.Others);
            parameters.Add("@Reserve", entity.Reserve);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_InsertMonthlyFinancial",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return parameters.Get<int>("@Id");
        }

        public async Task<MonthlyFinancialControl> UpdateAsync(MonthlyFinancialControl entity)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(
                "sp_UpdateMonthlyFinancial",
                new
                {
                    entity.Id,
                    entity.UserId,
                    entity.Year,
                    entity.Month,
                    entity.Money,
                    entity.RV,
                    entity.Debit,
                    entity.Others,
                    entity.Reserve
                },
                commandType: CommandType.StoredProcedure
            );

            return entity;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);

            var rows = await connection.ExecuteAsync(
                "sp_DeleteMonthlyFinancial",
                new { Id = id, UserId = userId },
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

        public async Task<MonthlyFinancialControl> GetByIdAsync(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<MonthlyFinancialControl>(
                "sp_GetMonthlyFinancialById",
                new { Id = id, UserId = userId },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<List<MonthlyFinancialControl>> GetAllAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryAsync<MonthlyFinancialControl>(
                "sp_GetAllMonthlyFinancial",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<List<MonthlyFinancialControl>> GetByUserIdAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);

            // ? Busca apenas do usuário (ou tudo se for Admin - SP decide)
            var result = await connection.QueryAsync<MonthlyFinancialControl>(
                "sp_GetMonthlyFinancialByUserId",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<MonthlyFinancialControl> GetByYearAndMonthAsync(int userId, int year, int month)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<MonthlyFinancialControl>(
                "sp_GetMonthlyFinancialByYearMonth",
                new { UserId = userId, Year = year, Month = month },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<decimal> GetExpensesTotalByYearAndMonthAsync(int userId, int year, int month)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<decimal>(
                "sp_GetExpensesTotalByYearAndMonth",
                new { UserId = userId, Year = year, Month = month },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
    }
}

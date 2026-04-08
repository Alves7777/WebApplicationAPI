using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using WebApplicationAPI.Models;
using WebApplicationAPI.Repositories.Interfaces;

namespace WebApplicationAPI.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        async Task<IEnumerable<Category>> ICategoryRepository.GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryAsync<Category>(
                sql: "sp_GetAllCategories",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<Category>(
               "sp_GetCategoryById",
               new { Id = id },
               commandType: CommandType.StoredProcedure
           );
        }

        public async Task<int> CreateAsync(Category category)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.ExecuteScalarAsync<int>(
               "sp_CreateCategory",
               new { Name = category.Name, Description = category.Description, IsActive = category.IsActive },
               commandType: CommandType.StoredProcedure
           );
        }
    }
 }

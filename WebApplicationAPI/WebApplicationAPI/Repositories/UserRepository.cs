using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApplicationAPI.Models;
using WebApplicationAPI.Repositories.Interfaces;

namespace WebApplicationAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // sp_GetAllUsers
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            
            return await connection.QueryAsync<User>(
                "sp_GetAllUsers",
                commandType: CommandType.StoredProcedure
            );
        }

        // sp_GetUserById
        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            
             User test =  await connection.QueryFirstOrDefaultAsync<User>(
                "sp_GetUserById",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            return test;
        }

        // sp_GetUserByEmail
        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            
            return await connection.QueryFirstOrDefaultAsync<User>(
                "sp_GetUserByEmail",
                new { Email = email },
                commandType: CommandType.StoredProcedure
            );
        }

        // sp_CreateUser
        public async Task<int> CreateAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var result = await connection.QuerySingleAsync<int>(
                "sp_CreateUser",
                new { Name = user.Name, Email = user.Email },
                commandType: CommandType.StoredProcedure
            );
            
            return result;
        }

        // sp_UpdateUser
        public async Task<bool> UpdateAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var rowsAffected = await connection.ExecuteScalarAsync<int>(
                "sp_UpdateUser",
                new { Id = user.Id, Name = user.Name, Email = user.Email },
                commandType: CommandType.StoredProcedure
            );
            
            return rowsAffected > 0;
        }

        // sp_DeleteUser
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var rowsAffected = await connection.ExecuteScalarAsync<int>(
                "sp_DeleteUser",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
            
            return rowsAffected > 0;
        }

        // sp_SearchUsers
        public async Task<(IEnumerable<User> Users, int TotalCount)> SearchUsersAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var results = await connection.QueryAsync<UserWithCount>(
                "sp_SearchUsers",
                new { SearchTerm = searchTerm, PageNumber = pageNumber, PageSize = pageSize },
                commandType: CommandType.StoredProcedure
            );

            var users = results.Select(r => new User
            {
                Id = r.Id,
                Name = r.Name,
                Email = r.Email
            });

            var totalCount = results.FirstOrDefault()?.TotalCount ?? 0;

            return (users, totalCount);
        }

        // Classe auxiliar para receber dados com TotalCount da procedure
        private class UserWithCount
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public int TotalCount { get; set; }
        }
    }
}

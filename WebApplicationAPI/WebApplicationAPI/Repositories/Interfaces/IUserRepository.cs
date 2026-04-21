using WebApplicationAPI.Models;

namespace WebApplicationAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(int requestUserId); // ? Adicionado requestUserId
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<int> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<(IEnumerable<User> Users, int TotalCount)> SearchUsersAsync(string? searchTerm, int pageNumber, int pageSize);
    }
}

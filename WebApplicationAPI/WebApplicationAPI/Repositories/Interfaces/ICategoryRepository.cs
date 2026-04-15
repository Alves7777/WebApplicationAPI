using WebApplicationAPI.Models;

namespace WebApplicationAPI.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<IEnumerable<Category>> GetByUserIdAsync(int userId); // ✅ Novo método
        Task<Category?> GetByIdAsync(int id);
        Task<int> CreateAsync(Category category);
    }
}

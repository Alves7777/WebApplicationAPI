using WebApplicationAPI.DTO;
using WebApplicationAPI.DTO;

namespace WebApplicationAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoryAsync(int userId); // ✅ Adicionado userId

        Task<CategoryResponse> GetCategoryByIdAsync(int id, int userId); // ✅ Adicionado userId

        Task<CategoryResponse> CreateCategoryAsync(int userId, CreateCategoryRequest request); // ✅ Adicionado userId

    }
}

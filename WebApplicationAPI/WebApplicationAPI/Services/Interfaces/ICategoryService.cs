using WebApplicationAPI.DTO;
using WebApplicationAPI.DTO;

namespace WebApplicationAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoryAsync();

        Task<CategoryResponse> GetCategoryByIdAsync(int id);

        Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);

    }
}

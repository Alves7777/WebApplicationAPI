using WebApplicationAPI.DTO;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Models;
using WebApplicationAPI.Repositories;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Services.Interfaces;

namespace WebApplicationAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoryAsync(int userId)
        {
            // ✅ Busca apenas categorias do usuário logado
            var category = await _categoryRepository.GetByUserIdAsync(userId);
            return category.Select(query => new CategoryResponse
            {
                Id = query.Id,
                Name = query.Name,
                Description = query.Description,
                IsActive = query.IsActive
            });
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(int id, int userId)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category is null) {
                return null;
            }

            // ✅ Verificar se a categoria pertence ao usuário
            if (category.UserId != userId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para acessar esta categoria");
            }

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
            };
        }

        public async Task<CategoryResponse> CreateCategoryAsync(int userId, CreateCategoryRequest request)
        {
            var category = new Category
            {
                UserId = userId, // ✅ Vincula ao usuário logado
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            var id = await _categoryRepository.CreateAsync(category);
            category.Id = id;

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }
    }
}

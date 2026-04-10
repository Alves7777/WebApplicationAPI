using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Services;
using WebApplicationAPI.Services.Interfaces;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoryAsync();
                return Ok(ApiResponse<IEnumerable<CategoryResponse>>.Success(categories));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);

                if (category == null) {
                    return NotFound(ApiResponse<object>.Fail($"Categoria com ID {id} não encontrada"));
                }

                return Ok(ApiResponse<CategoryResponse>.Success(category));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CategoryResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var category = await _categoryService.CreateCategoryAsync(request);
                return StatusCode(201, ApiResponse<CategoryResponse>.Success(category, "Categoria criada com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }
    }
}

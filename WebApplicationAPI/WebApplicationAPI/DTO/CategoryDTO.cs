using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationAPI.DTO
{
    public class CreateCategoryRequest
    {
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }

    public class UpdateCategoryRequest
    {
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }

    public class CategoryResponse
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

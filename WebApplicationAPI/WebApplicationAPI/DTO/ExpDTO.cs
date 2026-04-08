using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace WebApplicationAPI.DTO
{
    public class CreateExpRequest
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que 0.")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpenseDate { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "O mes deve estar entre 1 e 12.")]
        public int Month { get; set; }

        [Required]
        [Range(2000, int.MaxValue, ErrorMessage = "O ano deve ser posterior a 2000.")]
        public int Year { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Status { get; set; }

        [Required]
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }
    }

    public class UpdateExpRequest
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que 0.")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpenseDate { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "O mes deve estar entre 1 e 12.")]
        public int Month { get; set; }

        [Required]
        [Range(2000, int.MaxValue, ErrorMessage = "O ano deve ser posterior a 2000.")]
        public int Year { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Status { get; set; }

        [Required]
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }
    }

    public class ExpResponse
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime ExpenseDate { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

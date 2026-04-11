using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationAPI.DTO
{
    public class CreateCreditCardRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string? Brand { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CardLimit { get; set; }

        [Range(1, 31)]
        public int? ClosingDay { get; set; }

        [Range(1, 31)]
        public int? DueDay { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateCreditCardRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string? Brand { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CardLimit { get; set; }

        [Range(1, 31)]
        public int? ClosingDay { get; set; }

        [Range(1, 31)]
        public int? DueDay { get; set; }

        public bool IsActive { get; set; }
    }

    public class CreditCardResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Brand { get; set; }
        public decimal CardLimit { get; set; }
        public int? ClosingDay { get; set; }
        public int? DueDay { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateCreditCardExpenseRequest
    {
        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "PENDENTE";
    }

    public class UpdateCreditCardExpenseRequest
    {
        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }
    }

    public class CreditCardExpenseResponse
    {
        public int Id { get; set; }
        public int CreditCardId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string? Category { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CsvImportResult
    {
        public int TotalRecords { get; set; }
        public int ImportedRecords { get; set; }
        public int DuplicateCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public Dictionary<string, int> CategoriesCount { get; set; } = new Dictionary<string, int>();
    }

    public class CreditCardStatementResponse
    {
        public string CardName { get; set; }
        public string? Brand { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime? BillingPeriodStart { get; set; }
        public DateTime? BillingPeriodEnd { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalTransactions { get; set; }
        public decimal CardLimit { get; set; }
        public decimal AvailableLimit => CardLimit - TotalAmount;
        public decimal UsagePercentage => CardLimit > 0 ? (TotalAmount / CardLimit) * 100 : 0;
        public List<CreditCardExpenseResponse> Expenses { get; set; } = new List<CreditCardExpenseResponse>();
    }

    public class CategoryAnalysisResponse
    {
        public string Category { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageAmount { get; set; }
        public decimal Percentage { get; set; }
    }
}

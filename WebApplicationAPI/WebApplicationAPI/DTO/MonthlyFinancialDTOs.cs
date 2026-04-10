using System.ComponentModel.DataAnnotations;

namespace WebApplicationAPI.DTO
{
    public class CreateMonthlyFinancialRequest
    {
        [Required]
        [Range(2020, 2100)]
        public int Year { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Money { get; set; }

        [Range(0, double.MaxValue)]
        public decimal RV { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Debit { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Others { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Reserve { get; set; }
    }

    public class UpdateMonthlyFinancialRequest
    {
        [Required]
        [Range(2020, 2100)]
        public int Year { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Money { get; set; }

        [Range(0, double.MaxValue)]
        public decimal RV { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Debit { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Others { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Reserve { get; set; }
    }

    public class MonthlyFinancialResponse
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal Money { get; set; }
        public decimal RV { get; set; }
        public decimal Debit { get; set; }
        public decimal Others { get; set; }
        public decimal Reserve { get; set; }
        public decimal SalaryTotal { get; set; }
        public decimal ExpensesTotal { get; set; }
        public decimal Balance { get; set; }
        public decimal CanSpend { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

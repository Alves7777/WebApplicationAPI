namespace WebApplicationAPI.DTO
{
    public class CreateExpenseRequest
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class UpdateExpenseRequest
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class PatchExpenseRequest
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class ExpenseResponse
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SummaryResponse
    {
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }
        public decimal AvailableToSpend { get; set; }
        public Dictionary<string, decimal> TotalByCategory { get; set; }
        public Dictionary<string, decimal> TotalByStatus { get; set; }
    }
}
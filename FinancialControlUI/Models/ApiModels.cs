namespace FinancialControlUI.Models;

public class ApiResponse<T>
{
    public string Status { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}

public class ApiResponse
{
    public string Status { get; set; }
    public string? Message { get; set; }
}

public class CreditCard
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Brand { get; set; }
    public decimal CardLimit { get; set; }
    public int? ClosingDay { get; set; }
    public int? DueDay { get; set; }
    public bool IsActive { get; set; }
}

public class CreateCreditCardRequest
{
    public string Name { get; set; }
    public string? Brand { get; set; }
    public decimal CardLimit { get; set; }
    public int? ClosingDay { get; set; }
    public int? DueDay { get; set; }
}

public class UpdateCreditCardRequest
{
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public decimal? CardLimit { get; set; }
    public int? ClosingDay { get; set; }
    public int? DueDay { get; set; }
    public bool? IsActive { get; set; }
}

public class SimulatePurchaseRequest
{
    public decimal Amount { get; set; }
    public int Installments { get; set; }
    public string Description { get; set; }
}

public class MonthAnalysis
{
    public int Month { get; set; }
    public int Year { get; set; }
    public string MonthName { get; set; }
    public DateTime BillingPeriodStart { get; set; }
    public DateTime BillingPeriodEnd { get; set; }
    public decimal Salary { get; set; }
    public decimal CreditCardExpenses { get; set; }
    public decimal OtherExpenses { get; set; }
    public decimal CurrentExpenses { get; set; }
    public decimal Reserve { get; set; }
    public decimal ExistingInstallments { get; set; }
    public decimal NewInstallment { get; set; }
    public decimal TotalCommitment { get; set; }
    public decimal AvailableAfter { get; set; }
    public string Status { get; set; }
    public string? Alert { get; set; }
    public bool HasMonthlyControl { get; set; }
}

public class SimulationSummary
{
    public decimal TotalImpact { get; set; }
    public decimal MonthlyImpact { get; set; }
    public int RiskMonthsCount { get; set; }
    public int SafeMonthsCount { get; set; }
    public List<string> RiskMonths { get; set; } = new();
    public decimal AverageSalary { get; set; }
    public decimal AverageExpenses { get; set; }
    public decimal AverageAvailable { get; set; }
}

public class SimulatePurchaseResponse
{
    public bool CanAfford { get; set; }
    public string Recommendation { get; set; }
    public decimal PurchaseAmount { get; set; }
    public decimal InstallmentAmount { get; set; }
    public int Installments { get; set; }
    public List<MonthAnalysis> Analysis { get; set; } = new();
    public SimulationSummary Summary { get; set; }
}

public class MonthlyFinancial
{
    public int Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; }
    public decimal Money { get; set; }
    public decimal SalaryTotal { get; set; }
    public decimal ExpensesTotal { get; set; }
    public decimal Balance { get; set; }
    public decimal CanSpend { get; set; }
}

public class CreateMonthlyFinancialRequest
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Money { get; set; }
}

public class UpdateMonthlyFinancialRequest
{
    public decimal? Money { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCategoryRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Expense
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

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class CreateUserRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class UpdateUserRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class FinancialSummary
{
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
    public decimal AvailableToSpend { get; set; }
    public Dictionary<string, decimal> TotalByCategory { get; set; }
    public Dictionary<string, decimal> TotalByStatus { get; set; }
}

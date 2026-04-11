using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationAPI.DTO
{
    public class SimulatePurchaseRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public decimal Amount { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Parcelas devem estar entre 1 e 12")]
        public int Installments { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
    }

    public class SimulatePurchaseResponse
    {
        public bool CanAfford { get; set; }
        public string Recommendation { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal InstallmentAmount { get; set; }
        public int Installments { get; set; }
        public List<MonthAnalysis> Analysis { get; set; } = new List<MonthAnalysis>();
        public SimulationSummary Summary { get; set; }
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
        public decimal ProjectedExpenses { get; set; }
        public decimal Reserve { get; set; }
        public decimal ExistingInstallments { get; set; }
        public decimal NewInstallment { get; set; }
        public decimal TotalCommitment { get; set; }
        public decimal AvailableAfter { get; set; }
        public string Status { get; set; }
        public string Alert { get; set; }
        public bool HasMonthlyControl { get; set; }
    }

    public class SimulationSummary
    {
        public decimal TotalImpact { get; set; }
        public decimal MonthlyImpact { get; set; }
        public int RiskMonthsCount { get; set; }
        public int SafeMonthsCount { get; set; }
        public List<string> RiskMonths { get; set; } = new List<string>();
        public decimal AverageSalary { get; set; }
        public decimal AverageExpenses { get; set; }
        public decimal AverageAvailable { get; set; }
    }

    public class ConfirmPurchaseRequest
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [Range(1, 24)]
        public int Installments { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
    }

    public class InstallmentPurchaseResponse
    {
        public int Id { get; set; }
        public int CreditCardId { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
        public int InstallmentCount { get; set; }
        public decimal InstallmentAmount { get; set; }
        public int FirstInstallmentMonth { get; set; }
        public int FirstInstallmentYear { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RemainingInstallments { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}

using System;

namespace WebApplicationAPI.Models
{
    public class MonthlyFinancialControl
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Money { get; set; }
        public decimal RV { get; set; }
        public decimal Debit { get; set; }
        public decimal Others { get; set; }
        public decimal Reserve { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

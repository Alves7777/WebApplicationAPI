using System;

namespace WebApplicationAPI.Models
{
    public class CreditCardExpense
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
}

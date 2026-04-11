using System;

namespace WebApplicationAPI.Models
{
    public class InstallmentPurchase
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
    }
}

using System;

namespace WebApplicationAPI.Models
{
    public class CreditCard
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string? Brand { get; set; }
        public decimal CardLimit { get; set; }
        public int? ClosingDay { get; set; }
        public int? DueDay { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }
}

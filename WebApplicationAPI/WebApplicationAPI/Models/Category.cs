namespace WebApplicationAPI.Models
{
    public class Category
    {
        public int Id { get; set; }

        public int UserId { get; set; } // ✅ Adicionado para multi-tenancy

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

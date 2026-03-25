using System.ComponentModel.DataAnnotations;

namespace WebApplicationAPI.DTO
{
    public class UserDTO
    {
        [Required]
        public required int Id { get; set; }

        public string Name { get; set; }

        [EmailAddress]
        [Required]
        public required string Email { get; set; }
    }
}

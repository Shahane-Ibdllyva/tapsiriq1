using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Role : BaseEntity
    {
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty; // Məs: "Admin", "User"

        [MaxLength(250)]
        public string Description { get; set; } = string.Empty; // Rolun qısa təsviri

        // Many-to-Many əlaqə üçün Navigation Property
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tapsiriq1.Domain.Entities;

namespace Domain.Models
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; } 

     
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }

       
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        [ForeignKey(nameof(AppFile))]
        public Guid? ImageId { get; set; }

        public AppFile? AppFile { get; set; }
    }
}
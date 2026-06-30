using System.Collections.Generic;

namespace Domain.Models
{
    public class Department : BaseEntity
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        // Xarici açar (Foreign Key) Organization üçün
        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        // Navigation Property: Bir şöbədə çoxlu istifadəçi olar
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
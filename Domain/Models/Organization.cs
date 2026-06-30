using System.Collections.Generic;

namespace Domain.Models
{
    public class Organization : BaseEntity
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Navigation Property: Bir təşkilatın çoxlu şöbəsi olar
        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
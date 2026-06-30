using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Department
{
    public class CreateDepartmentDto
    {
      public string DepartmentName { get; set; } = string.Empty;
        public int OrganizationId { get; set; }
    }
}
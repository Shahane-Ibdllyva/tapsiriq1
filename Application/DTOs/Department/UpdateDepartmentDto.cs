namespace Application.DTOs.Department
{
    public class UpdateDepartmentDto
    {
        public int DepartmentId { get; set; } 
        public string DepartmentName { get; set; } = string.Empty;
        public int OrganizationId { get; set; }
    }
}
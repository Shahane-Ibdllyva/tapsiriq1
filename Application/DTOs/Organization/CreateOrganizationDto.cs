namespace Application.DTOs.Organization
{
    
    public class CreateOrganizationDto
    {
        public string OrganizationName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    
    public class UpdateOrganizationDto
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

  
    public class OrganizationDto
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Əgər departamentləri də siyahıda bəsit formada göstərmək istəsəniz:
        // public List<string> DepartmentNames { get; set; } = new();
    }
}
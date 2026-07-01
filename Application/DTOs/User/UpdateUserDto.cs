using Microsoft.AspNetCore.Http;

namespace Application.DTOs.User
{
    public class UpdateUserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int OrganizationId { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.User
{
    public class CreateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int OrganizationId { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
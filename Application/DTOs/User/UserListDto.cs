namespace Application.DTOs.User
{
    public class UserListDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
     public int? OrganizationId { get; set; }
        public Guid? ImageId { get; set; }
    }
}
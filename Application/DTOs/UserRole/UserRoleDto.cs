using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserRole
{
    public class CreateUserRoleDto
    {
        
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }

    public class UpdateUserRoleDto
    {
        public int UserRoleId { get; set; }

      
        public int UserId { get; set; }

        public int RoleId { get; set; }

        
        public int Status { get; set; }
    }
    public class UserRoleListDto
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
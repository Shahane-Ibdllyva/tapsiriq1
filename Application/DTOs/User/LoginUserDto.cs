using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "İstifadəçi adı mütləq daxil edilməlidir!")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifrə mütləq daxil edilməlidir!")]
        public string Password { get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PasswordReset
{
    public class PasswordResetConfirmDto
    {
        [Required(ErrorMessage = "Təsdiq kodu (UTP) daxil edilməlidir.")]
        [MinLength(6, ErrorMessage = "Kod 6 rəqəmdən ibarət olmalıdır.")]
        [MaxLength(6, ErrorMessage = "Kod 6 rəqəmdən ibarət olmalıdır.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Yeni şifrə daxil edilməlidir.")]
        [MinLength(6, ErrorMessage = "Şifrə ən azı 6 simvol olmalıdır.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Şifrə təkrarı daxil edilməlidir.")]
        [Compare("NewPassword", ErrorMessage = "Şifrə və şifrə təkrarı eyni deyil.")]
        public string ConfirmNewPassword { get; set; }
    }
}
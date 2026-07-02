using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PasswordReset
{
    public class PasswordResetRequestDto
    {
        [Required(ErrorMessage = "Email ünvanı daxil edilməlidir.")]
        [EmailAddress(ErrorMessage = "Düzgün email formatı daxil edin.")]
        public string Email { get; set; }
    }
}
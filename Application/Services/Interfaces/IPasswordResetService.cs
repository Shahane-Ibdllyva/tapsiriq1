using System.Threading.Tasks;
using Application.DTOs.PasswordReset;

namespace Application.Services.Interfaces
{
    public interface IPasswordResetService
    {
        
        /// İstifadəçinin email ünvanına 6 rəqəmli UTP kodu göndərir
       Task<PasswordResetResponseDto> RequestPasswordResetAsync(PasswordResetRequestDto dto);

        /// UTP kodunu təsdiqləyir və yeni şifrə təyin edir
        Task<PasswordResetResponseDto> ConfirmResetPasswordAsync(PasswordResetConfirmDto dto);
    }
}
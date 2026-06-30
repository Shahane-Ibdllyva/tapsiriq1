using System.Threading.Tasks;
using Application.DTOs.Auth;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        //int? GetCurrentUserId();
    }
}
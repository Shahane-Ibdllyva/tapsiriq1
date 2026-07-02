using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.DTOs.PasswordReset;
using Application.Services.Interfaces;

namespace WebAPI.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous] 
    public class PasswordResetController : ControllerBase
    {
        private readonly IPasswordResetService _passwordResetService;

        public PasswordResetController(IPasswordResetService passwordResetService)
        {
            _passwordResetService = passwordResetService;
        }

        /// <summary>
        /// 1. Addım: Email ünvanına 6 rəqəmli UTP kodu göndər
        /// </summary>
        /// <param name="dto">İstifadəçinin email ünvanı</param>
        /// <returns>Əməliyyat nəticəsi</returns>
        [HttpPost("request")]
        [ProducesResponseType(typeof(PasswordResetResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto dto)
        {
            var result = await _passwordResetService.RequestPasswordResetAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// 2. Addım: UTP kodunu təsdiqlə və yeni şifrə təyin et
        /// </summary>
        /// <param name="dto">UTP kodu və yeni şifrə</param>
        /// <returns>Əməliyyat nəticəsi</returns>
        [HttpPost("confirm")]
        [ProducesResponseType(typeof(PasswordResetResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] PasswordResetConfirmDto dto)
        {
            var result = await _passwordResetService.ConfirmResetPasswordAsync(dto);
            return Ok(result);
        }
    }
}
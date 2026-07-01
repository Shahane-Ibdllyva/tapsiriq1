using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs.User;
using Application.Services.Interfaces;

namespace tapsiriq1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllUsersAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateUserDto dto) // Şəkil üçün [FromForm] əlavə olundu
        {
            // JWT Token-dən mövcud istifadəçinin ID-sini oxuyuruq (Custom claim handle məntiqinə uyğun)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim) : 1; // Token yoxdursa test üçün 1

            await _userService.CreateUserAsync(dto, currentUserId);
            return StatusCode(201, new { message = "İstifadəçi uğurla yaradıldı." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateUserDto dto) // Şəkil üçün [FromForm] əlavə olundu
        {
            dto.UserId = id;

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim) : 1;

            await _userService.UpdateUserAsync(dto, currentUserId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
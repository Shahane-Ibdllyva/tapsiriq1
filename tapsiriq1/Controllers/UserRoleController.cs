using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.DTOs.UserRole;
using Application.Services.Interfaces;

namespace tapsiriq1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRolesByUserId(int userId)
        {
            var result = await _userRoleService.GetRolesByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(CreateUserRoleDto dto)
        {
            await _userRoleService.AssignRoleToUserAsync(dto);
            return StatusCode(201, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserRole(int id, UpdateUserRoleDto dto)
        {
            dto.UserRoleId = id;
            await _userRoleService.UpdateUserRoleAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveRole(int id)
        {
            await _userRoleService.RemoveRoleFromUserAsync(id);
            return NoContent();
        }
    }
}
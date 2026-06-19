using Microsoft.AspNetCore.Mvc;
using System;
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

        // Müəyyən bir istifadəçinin bütün aktiv rollarını gətirir
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRolesByUserId(int userId)
        {
            try
            {
                var result = await _userRoleService.GetRolesByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // İstifadəçiyə rol təyin edir
        [HttpPost]
        public async Task<IActionResult> AssignRole(CreateUserRoleDto dto)
        {
            try
            {
                await _userRoleService.AssignRoleToUserAsync(dto);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // İstifadəçinin rol əlaqəsini yeniləyir (Məsələn statusunu)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserRole(int id, UpdateUserRoleDto dto)
        {
            try
            {
                await _userRoleService.UpdateUserRoleAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // İstifadəçidən rolu geri alır (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveRole(int id)
        {
            try
            {
                await _userRoleService.RemoveRoleFromUserAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            await _userService.CreateUserAsync(dto);
            // Həm 201 qaytarır, həm də Headers-də bu istifadəçinin linkini (Location) yaradır
            return CreatedAtAction(nameof(GetById), new { id = dto.Email }, dto);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateUserDto dto)
        {
            await _userService.UpdateUserAsync(id, dto);
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
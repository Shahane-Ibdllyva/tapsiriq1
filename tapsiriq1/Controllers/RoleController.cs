using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.DTOs.Role;
using Application.Services.Interfaces;

namespace tapsiriq1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roleService.GetAllRolesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // try-catch silindi, xəta olarsa ExceptionMiddleware avtomatik 404 qaytaracaq
            var result = await _roleService.GetRoleByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleDto dto)
        {
            await _roleService.CreateRoleAsync(dto);
            return StatusCode(201, dto); // 201 Created standartı
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateRoleDto dto)
        {
            // URL-dən gələn id-ni DTO daxilindəki RoleId-yə mənimsədirik
            dto.RoleId = id;

            await _roleService.UpdateRoleAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _roleService.DeleteRoleAsync(id);
            return NoContent();
        }
    }
}
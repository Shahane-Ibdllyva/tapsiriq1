using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.DTOs.Organization;
using Application.Services.Interfaces;

namespace tapsiriq1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _organizationService.GetAllOrganizationsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _organizationService.GetOrganizationByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrganizationDto dto)
        {
            await _organizationService.CreateOrganizationAsync(dto);
            return StatusCode(201, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateOrganizationDto dto)
        {
            // URL-dən gələn id-ni təhlükəsizlik və arxitektura uyğunluğu üçün DTO-nun daxilinə mənimsədirik
            dto.OrganizationId = id;

            await _organizationService.UpdateOrganizationAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _organizationService.DeleteOrganizationAsync(id);
            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System;
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

        // Controller sadəcə servisi tanıyır, repozitoriyanı yox!
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
            if (result == null) return NotFound("Təşkilat tapılmadı!");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrganizationDto dto)
        {
            try
            {
                await _organizationService.CreateOrganizationAsync(dto);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateOrganizationDto dto)
        {
            try
            {
                await _organizationService.UpdateOrganizationAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _organizationService.DeleteOrganizationAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
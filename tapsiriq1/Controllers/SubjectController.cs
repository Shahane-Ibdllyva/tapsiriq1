using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.DTOs.Subject;
using Application.Services.Interfaces;

namespace tapsiriq1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            return Ok(subjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            return Ok(subject);
        }

        [HttpGet("hardest")]
        public async Task<IActionResult> GetHardestSubjects([FromQuery] int passingGrade = 51)
        {
            var hardestSubjects = await _subjectService.GetHardestSubjectsAsync(passingGrade);
            return Ok(hardestSubjects);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSubjectDto dto)
        {
            await _subjectService.CreateSubjectAsync(dto);
            return StatusCode(201, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSubjectDto dto)
        {
          
            dto.SubjectId = id;

            await _subjectService.UpdateSubjectAsync(dto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _subjectService.DeleteSubjectAsync(id);
            return NoContent();
        }
    }
}
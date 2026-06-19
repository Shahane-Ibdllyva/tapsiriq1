using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Domain.Models;
using Application.Services.Interfaces;

namespace tapsiriq1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        // Repozitoriya getdi, yerinə Servis gəldi
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
            if (subject == null) return NotFound("Fənn tapılmadı!");
            return Ok(subject);
        }

        [HttpGet("hardest")]
        public async Task<IActionResult> GetHardestSubjects([FromQuery] int passingGrade = 51)
        {
            var hardestSubjects = await _subjectService.GetHardestSubjectsAsync(passingGrade);
            return Ok(hardestSubjects);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Subject subject)
        {
            await _subjectService.CreateSubjectAsync(subject);
            return Ok(subject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Subject dto)
        {
            try
            {
                await _subjectService.UpdateSubjectAsync(id, dto);
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
                await _subjectService.DeleteSubjectAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
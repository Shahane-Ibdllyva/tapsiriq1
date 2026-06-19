using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Application.DTOs.Exam;
using Domain.Models;
using Application.Services.Interfaces;

namespace tapsiriq1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        // Repozitoriya tamamilə çıxdı, yerinə Servis gəldi
        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exams = await _examService.GetAllExamsAsync();
            return Ok(exams);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exam = await _examService.GetExamByIdAsync(id);
            if (exam == null) return NotFound("İmtahan tapılmadı!");
            return Ok(exam);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExamDto exam)
        {
            await _examService.CreateExamAsync(exam);
            return Ok(exam);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateExamDto dto)
        {
            try
            {
                await _examService.UpdateExamAsync(id, dto);
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
                await _examService.DeleteExamAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
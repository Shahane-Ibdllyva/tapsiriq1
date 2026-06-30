using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.DTOs.Exam;
using Application.Services.Interfaces;

namespace tapsiriq1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

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
            if (exam == null)
                return NotFound($"ID-si {id} olan imtahan tapılmadı!");

            return Ok(exam);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExamDto dto)
        {
            await _examService.CreateExamAsync(dto);
            return StatusCode(201, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateExamDto dto)
        {
            dto.ExamId = id; // URL-dəki ID-ni DTO-ya mənimsədirik
            await _examService.UpdateExamAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _examService.DeleteExamAsync(id);
            return NoContent();
        }
    }
}
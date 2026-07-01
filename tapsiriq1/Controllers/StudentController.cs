using Microsoft.AspNetCore.Authorization;
using Application.DTOs.Student;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Threading.Tasks;

namespace tapsiriq1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        
        [HttpGet("student-exams")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableQuery]
        public IActionResult GetStudentExamReports()
        {
            var query = _studentService.GetStudentExamReportsQueryable();
            return Ok(query);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students); 
        }

        [HttpGet("top/{count}")]
        public async Task<IActionResult> GetTopStudents(int count)
        {
            var topStudents = await _studentService.GetTopStudentsAsync(count);
            return Ok(topStudents); 
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetAllexam([FromQuery] StudentExamReportFilter filter)
        {
            var result = await _studentService.GetStudentReportAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);

            // Diqqət: Əgər servis daxilində tapılmayanda NotFoundException atırıqsa, 
            // əslində bu if yoxlamasına da ehtiyac qalmır, birbaşa return Ok(student) yaza bilərsən.
            if (student == null)
                return NotFound("Tələbə tapılmadı!");

            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStudentDto dto) 
        {
            await _studentService.CreateStudentAsync(dto);
            return StatusCode(201, dto); 
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateStudentDto dto) 
        {
            
            await _studentService.UpdateStudentAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            return NoContent();
        }
    }
}
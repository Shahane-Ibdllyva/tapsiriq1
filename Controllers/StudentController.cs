using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using Infrastructure.Data;
using Application.DTOs.Student;
using Domain.Models;
using Domain.Models.View;
using Application.Repositories;
using Application.Services.Interfaces;


namespace tapsiriq1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {

        private readonly IStudentService _studentService;

        // Controller artıq repozitoriyaları yox, sadəcə TƏK BİR servisi qəbul edir
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
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
            // Bütün o uzun if-lər getdi, birbaşa servisi çağırırıq:
            var result = await _studentService.GetStudentReportAsync(filter);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound("Tələbə tapılmadı!");
            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            await _studentService.CreateStudentAsync(student);
            return Ok(student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Student dto)
        {
            try
            {
                await _studentService.UpdateStudentAsync(id, dto);
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
                await _studentService.DeleteStudentAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
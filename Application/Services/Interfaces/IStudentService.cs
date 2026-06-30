using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Student;
using Domain.Models.View;

namespace Application.Services.Interfaces
{
    public interface IStudentService
    {
        IQueryable<StudentExamReport> GetStudentExamReportsQueryable();
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
        Task<StudentDto?> GetStudentByIdAsync(int id);
        Task<IEnumerable<StudentDto>> GetTopStudentsAsync(int count);
        Task<IEnumerable<StudentExamReport>> GetStudentReportAsync(StudentExamReportFilter filter);
        Task CreateStudentAsync(CreateStudentDto dto);
        Task UpdateStudentAsync(int id, UpdateStudentDto dto); 
        Task DeleteStudentAsync(int id);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Exam; // DTO-ların tanınması üçün vacibdir
using Domain.Models;

namespace Application.Services.Interfaces
{
    public interface IExamService
    {
        Task<IEnumerable<ExamDto>> GetAllExamsAsync();

        Task<ExamDto?> GetExamByIdAsync(int id);

        Task CreateExamAsync(CreateExamDto dto);
        Task UpdateExamAsync(UpdateExamDto dto);
        Task DeleteExamAsync(int id);
    }
}
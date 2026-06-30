using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Subject;

namespace Application.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync();
        Task<SubjectDto?> GetSubjectByIdAsync(int id);
        Task<IEnumerable<SubjectDto>> GetHardestSubjectsAsync(int passingGrade = 51);
        Task CreateSubjectAsync(CreateSubjectDto dto);
        Task UpdateSubjectAsync(UpdateSubjectDto dto);
        Task DeleteSubjectAsync(int id);
    }
}
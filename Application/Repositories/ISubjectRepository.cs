using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Repositories
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        Task<bool> ExistsAsync(int SubjectId);
        Task<IEnumerable<Subject>> GetHardestSubjectsAsync(int passingGrade = 51);

        Task<IEnumerable<Subject>> GetAllSubjectsListAsync();
    }
}
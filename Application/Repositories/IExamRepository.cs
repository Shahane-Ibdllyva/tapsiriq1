using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Repositories
{
    public interface IExamRepository : IRepository<Exam>
    {
      
        Task<bool> ExistsAsync(int ExamId);
        Task<IEnumerable<Exam>> GetExamsWithDetailsAsync();
        Task<Exam?> GetExamByIdWithDetailsAsync(int id);
    }
}
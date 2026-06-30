using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Student; // Süzgəc (Filter) modelinin olduğu yer
using Domain.Models.View;

namespace Application.Repositories
{
    public interface IViewRepository
    {
        IQueryable<StudentExamReport> GetStudentExamReportsQueryable();

        Task<IEnumerable<StudentExamReport>> GetFilteredReportsAsync(StudentExamReportFilter filter);
    }
}
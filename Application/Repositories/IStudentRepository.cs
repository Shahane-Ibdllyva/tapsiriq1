using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<bool> ExistsAsync(int StudentId);
     
        Task<IEnumerable<Student>> GetTopStudentsAsync(int count);

        Task<IEnumerable<Student>> GetAllStudentsListAsync();
     
        Task<bool> IsStudentExistsByNameAsync(string fullName);

    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Department;
using Domain.Models;

namespace Application.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<bool> ExistsAsync(int DepartmentId);
        Task<IEnumerable<DepartmentListDto>> GetActiveDepartmentsDtoAsync();
        Task<Department?> GetDepartmentWithDetailsAsync(int id);
        Task<bool> CheckDuplicateAsync(int organizationId, string name);
        Task<bool> CheckDuplicateForUpdateAsync(int organizationId, string name, int excludeId);
        Task<bool> CheckDepartmentExistsAsync(int? departmentId);
    }
}
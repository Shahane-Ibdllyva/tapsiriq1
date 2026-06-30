using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Department;
using Domain.Models;

namespace Application.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentListDto>> GetAllDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task CreateDepartmentAsync(CreateDepartmentDto dto);
        Task UpdateDepartmentAsync(UpdateDepartmentDto dto);
        Task DeleteDepartmentAsync(int id);
    }
}
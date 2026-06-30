using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Department;
using Application.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(AppDbContext context) : base(context)
        {
           
        }

        public async Task<IEnumerable<DepartmentListDto>> GetActiveDepartmentsDtoAsync()
        {
            return await _context.Departments
                .Where(d => d.Status == EntityStatus.Active)
                .Select(d => new DepartmentListDto
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    OrganizationId = d.OrganizationId
                })
                .ToListAsync();
        }

        public async Task<Department?> GetDepartmentWithDetailsAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Organization)
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
        }

        public async Task<bool> CheckDuplicateAsync(int organizationId, string name)
        {
            string cleanedName = name.Trim().ToLower();

            return await _context.Departments
                .AnyAsync(d => d.OrganizationId == organizationId &&
                               d.DepartmentName.Trim().ToLower() == cleanedName);
        }

        public async Task<bool> CheckDuplicateForUpdateAsync(int organizationId, string departmentName, int departmentId)
        {
            string cleanedName = departmentName.Trim().ToLower();

          
            return await _context.Departments
                .AnyAsync(d => d.OrganizationId == organizationId
                               && d.DepartmentName.Trim().ToLower() == cleanedName
                               && d.DepartmentId == departmentId
                               && d.Status != EntityStatus.Deleted);
        }

        public async Task<bool> CheckDepartmentExistsAsync(int? departmentId)
        {
            if (!departmentId.HasValue) return false;
            return await _context.Departments.AnyAsync(d => d.DepartmentId == departmentId.Value);
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Departments.AnyAsync(s => s.DepartmentId == id && s.Status != EntityStatus.Deleted);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
    {
       

        public OrganizationRepository(AppDbContext context) : base(context)
        {
           
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Organizations
                .AnyAsync(o => o.OrganizationId == id);
        }

        public async Task<IEnumerable<Organization>> GetOrganizationsWithDepartmentsAsync()
        {
            return await _context.Organizations
                .Include(o => o.Departments)
                .ToListAsync();
        }

        public async Task<Organization?> GetOrganizationByIdWithDepartmentsAsync(int id)
        {
            return await _context.Organizations
                .Include(o => o.Departments)
                .FirstOrDefaultAsync(o => o.OrganizationId == id);
        }

        public async Task<bool> CheckDuplicateNameAsync(string name)
        {
            return await _context.Organizations
                .AnyAsync(o => o.OrganizationName.ToLower() == name.ToLower());
        }

       
        public async Task<bool> CheckDuplicateNameForUpdateAsync(string name, int excludeId)
        {
            return await _context.Organizations
                .AnyAsync(o => o.OrganizationName.ToLower() == name.ToLower()
                               && o.OrganizationId != excludeId
                                && o.Status != EntityStatus.Deleted);
        }
    }
}
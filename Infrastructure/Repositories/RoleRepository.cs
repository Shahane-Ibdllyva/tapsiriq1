using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Role;
using Application.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore; 

namespace Infrastructure.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
      

        public RoleRepository(AppDbContext context) : base(context)
        {
          
        }

        public async Task<IEnumerable<RoleListDto>> GetActiveRolesDtoAsync()
        {
            return await _context.Roles
                .Where(r => r.Status == EntityStatus.Active)
                .Select(r => new RoleListDto
                {
                    RoleId = r.RoleId,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync();
        }

        public async Task<Role?> GetRoleWithDetailsAsync(int id)
        {
            return await _context.Roles
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.RoleId == id);
        }

        public async Task<bool> CheckDuplicateAsync(string name)
        {
            return await _context.Roles
                .AnyAsync(r => r.Name.ToLower() == name.ToLower() && r.Status == EntityStatus.Active);
        }

        public async Task<bool> CheckDuplicateForUpdateAsync(string name, int excludeId)
        {
            return await _context.Roles
                .AnyAsync(r => r.Name.ToLower() == name.ToLower() &&
                               r.RoleId != excludeId &&
                               r.Status == EntityStatus.Active);
        }

        public async Task<bool> CheckActiveRoleExistsAsync(int roleId)
        {
            return await _context.Roles.AnyAsync(r => r.RoleId == roleId && r.Status == EntityStatus.Active);
        }
    }
}
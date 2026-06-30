using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Domain.Models;
using Application.Repositories;
using Application.DTOs.UserRole;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
     

        public UserRoleRepository(AppDbContext context) : base(context)
        {
           
        }

        public async Task<List<Role>> GetRolesByUserIdAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserRoleListDto>> GetUserRolesDtoByUserIdAsync(int userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId && ur.Status == EntityStatus.Active)
                .Select(ur => new UserRoleListDto
                {
                    UserRoleId = ur.UserRoleId,
                    UserId = ur.UserId,
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.Name
                })
                .ToListAsync();
        }

       
        public async Task<UserRole?> GetByUserIdAndRoleIdAsync(int userId, int roleId)
        {
            return await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }

        
        public async Task<bool> CheckDuplicateForUpdateAsync(int userId, int roleId, int excludeId)
        {
            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId &&
                               ur.RoleId == roleId &&
                               ur.UserRoleId != excludeId &&
                               ur.Status == EntityStatus.Active);
        }
    }
}
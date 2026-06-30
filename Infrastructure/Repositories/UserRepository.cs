using Infrastructure.Data;
using Domain.Models;
using Application.Repositories;
using Application.DTOs.User;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
       

        public UserRepository(AppDbContext context) : base(context)
        {
            
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> CheckActiveUserExistsAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.UserId == userId && u.Status == EntityStatus.Active);
        }

     
        public async Task<IEnumerable<UserListDto>> GetAllActiveUsersDtoAsync()
        {
            return await _context.Users
                .Where(u => u.Status == EntityStatus.Active)
                .Select(u => new UserListDto
                {
                    UserId = u.UserId,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    Username = u.Username,
                    DepartmentId = u.DepartmentId,
                    OrganizationId = u.OrganizationId,
                })
                .ToListAsync();
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> CheckEmailExistsForUpdateAsync(string email, int excludeUserId)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.UserId != excludeUserId);
        }
    }
}
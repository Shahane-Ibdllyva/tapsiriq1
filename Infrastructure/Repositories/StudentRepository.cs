using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Domain.Models;
using Application.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
      
        public StudentRepository(AppDbContext context) : base(context)
        {
            
        }

        public async Task<IEnumerable<Student>> GetTopStudentsAsync(int count)
        {
            return await _context.Students
                .OrderByDescending(s => s.StudentId)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetAllStudentsListAsync()
        {
            
            return await _context.Students.ToListAsync();
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Students.AnyAsync(s => s.StudentId == id && s.Status != EntityStatus.Deleted);
        }
        public async Task<bool> IsStudentExistsByNameAsync(string fullName)
        {
            var searchName = fullName.Trim().ToLower();

            return await _context.Students
                .AnyAsync(s => (s.StudentName + " " + s.StudentSurname).ToLower().Contains(searchName) &&
                               s.Status == EntityStatus.Active);
        }
    }
}
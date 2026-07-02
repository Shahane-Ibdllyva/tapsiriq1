using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore; 

namespace Infrastructure.Repositories
{
    public class ExamRepository : Repository<Exam>, IExamRepository
    {
       

        public ExamRepository(AppDbContext context) : base(context)
        {
            
        }

       
        public async Task<IEnumerable<Exam>> GetExamsWithDetailsAsync()
        {
            return await _context.Exams
                .Include(e => e.Student)
                .Include(e => e.Subject)
                .ToListAsync();
        }

       
        public async Task<Exam?> GetExamByIdWithDetailsAsync(int id)
        {
            return await _context.Exams
                .Include(e => e.Student)
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.ExamId == id);
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Exams.AnyAsync(e => e.ExamId == id && e.Status != EntityStatus.Deleted);
        }
    }
}
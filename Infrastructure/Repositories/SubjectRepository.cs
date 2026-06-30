using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Domain.Models;
using Application.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
       

        public SubjectRepository(AppDbContext context) : base(context)
        {
           
        }

        public async Task<IEnumerable<Subject>> GetHardestSubjectsAsync(int passingGrade = 51)
        {
            var passedSubjectIds = await _context.Exams
                .Where(e => e.Grade >= passingGrade)
                .Select(e => e.SubjectId)
                .Distinct()
                .ToListAsync();

            return await _context.Subjects
                .Where(s => !passedSubjectIds.Contains(s.SubjectId))
                .ToListAsync();
        }

        
        public async Task<IEnumerable<Subject>> GetAllSubjectsListAsync()
        {
            return await _context.Subjects.ToListAsync();
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Subjects.AnyAsync(s => s.SubjectId == id && s.Status != EntityStatus.Deleted);
        }
    }
}
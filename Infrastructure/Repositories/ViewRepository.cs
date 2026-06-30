using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Domain.Models.View;
using Application.Repositories;
using Application.DTOs.Student; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ViewRepository : IViewRepository
    {
        private readonly AppDbContext _context;

        public ViewRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<StudentExamReport> GetStudentExamReportsQueryable()
        {
          
            return _context.StudentExamReports.AsNoTracking(); 
        }

        public IQueryable<StudentExamReport> GetAll()
        {
            return _context.StudentExamReports.AsNoTracking();
        }

        
        public async Task<IEnumerable<StudentExamReport>> GetFilteredReportsAsync(StudentExamReportFilter filter)
        {
            
            var query = GetAll();

            if (!string.IsNullOrWhiteSpace(filter.StudentFullName))
                query = query.Where(p => p.StudentFullName.StartsWith(filter.StudentFullName));

            if (!string.IsNullOrWhiteSpace(filter.SubjectName))
                query = query.Where(p => p.SubjectName.StartsWith(filter.SubjectName));

            if (!string.IsNullOrWhiteSpace(filter.TeacherFullName))
                query = query.Where(p => p.TeacherFullName.StartsWith(filter.TeacherFullName));

            if (filter.GradeMin.HasValue) query = query.Where(p => p.Grade >= filter.GradeMin.Value);
            if (filter.GradeMax.HasValue) query = query.Where(p => p.Grade <= filter.GradeMax.Value);

            if (filter.ExamDateMin.HasValue) query = query.Where(p => p.ExamDate >= filter.ExamDateMin.Value);
            if (filter.ExamDateMax.HasValue) query = query.Where(p => p.ExamDate <= filter.ExamDateMax.Value);

            // Bütün LINQ sorğusu tamamlandıqdan sonra bazaya asinxron sorğu atılır
            return await query.ToListAsync();
        }
    }
}
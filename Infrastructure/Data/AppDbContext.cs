using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // <--- Çox vacibdir: HttpContext-ə giriş üçün
using System.Security.Claims;    // <--- Çox vacibdir: Claims-ləri oxumaq üçün
using Domain.Models;
using Domain.Models.View;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Infrastructure.Data
{
    public partial class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor; // <--- 1. Bura sahə (field) əlavə edirik

        // 2. Konstruktorda IHttpContextAccessor-u qəbul edirik (Inject edirik)
        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentExamReport> StudentExamReports { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public override int SaveChanges()
        {
            ApplyAuditInformation();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return base.SaveChangesAsync(cancellationToken);
        }

        // 3. Mərkəzi Audit metodumuzu tamamilə dinamik edirik
        private void ApplyAuditInformation()
        {
            int currentUserId = 0; // default olaraq 0 (sistem istifadəçisi kimi)

            // Token-dən "userId" claim-ni oxuyuruq (AuthService-dəki ilə eyni)
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int id))
            {
                currentUserId = id; // token varsa, həmin ID istifadə olunur
            }

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.InsertDate = DateTime.UtcNow;
                    entry.Entity.Status = EntityStatus.Active;
                    entry.Entity.InsertByUserId = currentUserId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdateDate = DateTime.UtcNow;
                    entry.Entity.UpdateByUserId = currentUserId;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentExamReport>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("VW_STUDENT_EXAM_REPORT");
            });
        }
    }
}
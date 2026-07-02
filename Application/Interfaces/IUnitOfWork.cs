using System;
using System.Threading.Tasks;
using Application.Repositories;

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Mövcud repository-ləriniz
        IUserRepository Users { get; }
        IAppFileRepository AppFiles { get; }
        IDepartmentRepository Departments { get; }
        IOrganizationRepository Organizations { get; }
        IExamRepository Exams { get; }
        IStudentRepository Students { get; }          
        IRoleRepository Roles { get; }               
        IViewRepository StudentExamReports { get; }
        ISubjectRepository Subjects { get; }       
        IUserRoleRepository UserRoles { get; }
        IPasswordResetTokenRepository PasswordResetTokens { get; }


        // Transaction əməliyyatları
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;
        private bool _disposed;

        private IUserRepository _userRepository;
        private IAppFileRepository _appFileRepository;
        private IDepartmentRepository _departmentRepository;
        private IOrganizationRepository _organizationRepository;
        private IExamRepository _examRepository;
        private IStudentRepository _studentRepository;     
        private IRoleRepository _roleRepository;           
        private IViewRepository _viewRepository;
        private ISubjectRepository _subjectRepository;     
        private IUserRoleRepository _userRoleRepository;
        private IPasswordResetTokenRepository _passwordResetTokenRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users =>
            _userRepository ??= new UserRepository(_context);

        public IAppFileRepository AppFiles =>
            _appFileRepository ??= new AppFileRepository(_context);

        public IDepartmentRepository Departments =>
            _departmentRepository ??= new DepartmentRepository(_context);
        public IOrganizationRepository Organizations =>
          _organizationRepository ??= new OrganizationRepository(_context);
        public IExamRepository Exams =>
            _examRepository ??= new ExamRepository(_context);
        public IStudentRepository Students =>
           _studentRepository ??= new StudentRepository(_context); 

        public IRoleRepository Roles =>
            _roleRepository ??= new RoleRepository(_context);     

        public IViewRepository StudentExamReports =>
            _viewRepository ??= new ViewRepository(_context);
        public ISubjectRepository Subjects =>
            _subjectRepository ??= new SubjectRepository(_context); 

        public IUserRoleRepository UserRoles =>
            _userRoleRepository ??= new UserRoleRepository(_context);
        public IPasswordResetTokenRepository PasswordResetTokens =>
    _passwordResetTokenRepository ??= new PasswordResetTokenRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _transaction?.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction?.RollbackAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
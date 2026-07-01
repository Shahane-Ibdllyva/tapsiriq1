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
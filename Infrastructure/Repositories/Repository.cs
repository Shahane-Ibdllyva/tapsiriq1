using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Domain.Models;
using Application.Repositories;

namespace Infrastructure.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;
       

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
           
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public IQueryable<T> GetAll()
        {
            // AsNoTracking() performansı artırır, çünki sadəcə oxuma ssenarilərində EF Core obyekti izləmir
            return _dbSet.AsNoTracking();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            // Obyektin vəziyyətini Modified edirik, bazaya yazılma əmrini gözləyirik
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
    }
}
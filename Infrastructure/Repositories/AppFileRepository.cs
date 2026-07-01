using Application.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using tapsiriq1.Domain.Entities;

namespace Infrastructure.Repositories
{
    public class AppFileRepository : Repository<AppFile>, IAppFileRepository
    {
        public AppFileRepository(AppDbContext context) : base(context)
        {
           
        }
    }
}
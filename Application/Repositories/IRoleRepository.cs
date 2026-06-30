using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Role;
using Domain.Models;

namespace Application.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
    
        Task<IEnumerable<RoleListDto>> GetActiveRolesDtoAsync();
        Task<Role?> GetRoleWithDetailsAsync(int id);
        Task<bool> CheckDuplicateAsync(string name);
        Task<bool> CheckDuplicateForUpdateAsync(string name, int excludeId);
        Task<bool> CheckActiveRoleExistsAsync(int roleId);
    }
}
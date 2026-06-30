using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.UserRole;
using Domain.Models;

namespace Application.Repositories
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        Task<List<Role>> GetRolesByUserIdAsync(int userId);
        Task<IEnumerable<UserRoleListDto>> GetUserRolesDtoByUserIdAsync(int userId);
        Task<UserRole?> GetByUserIdAndRoleIdAsync(int userId, int roleId);
        Task<bool> CheckDuplicateForUpdateAsync(int userId, int roleId, int excludeId);
    }
}
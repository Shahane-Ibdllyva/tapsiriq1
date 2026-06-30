using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.UserRole;

namespace Application.Services.Interfaces
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRoleListDto>> GetRolesByUserIdAsync(int userId);
        Task AssignRoleToUserAsync(CreateUserRoleDto dto);
        Task UpdateUserRoleAsync(UpdateUserRoleDto dto);
        Task RemoveRoleFromUserAsync(int id);
    }
}
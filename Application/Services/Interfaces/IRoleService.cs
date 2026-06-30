using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Role;

namespace Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleListDto>> GetAllRolesAsync();
        Task<RoleListDto?> GetRoleByIdAsync(int id);
        Task CreateRoleAsync(CreateRoleDto dto);
        Task UpdateRoleAsync(UpdateRoleDto dto); 
        Task DeleteRoleAsync(int id);
    }
}
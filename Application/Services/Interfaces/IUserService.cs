using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.User;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserListDto>> GetAllUsersAsync();
        Task<UserListDto?> GetUserByIdAsync(int id);
        Task CreateUserAsync(CreateUserDto dto);
        Task UpdateUserAsync(UpdateUserDto dto); 
        Task DeleteUserAsync(int id);
    }
}
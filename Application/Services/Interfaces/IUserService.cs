using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.User;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserListDto>> GetAllUsersAsync();
        Task<UserListDto?> GetUserByIdAsync(int id);
        Task CreateUserAsync(CreateUserDto dto,int currentUserId);
        Task UpdateUserAsync(UpdateUserDto dto, int currentUserId); 
        Task DeleteUserAsync(int id);
    }
}
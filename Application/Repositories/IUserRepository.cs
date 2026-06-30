using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.User;
using Domain.Models;

namespace Application.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> CheckActiveUserExistsAsync(int userId);

       
        Task<IEnumerable<UserListDto>> GetAllActiveUsersDtoAsync();
        Task<bool> CheckEmailExistsAsync(string email);
        Task<bool> CheckEmailExistsForUpdateAsync(string email, int excludeUserId);
    }
}
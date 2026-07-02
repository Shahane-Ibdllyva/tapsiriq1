using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
    {
        Task<PasswordResetToken?> GetByTokenAsync(string token);
        Task<PasswordResetToken?> GetLastValidTokenByUserIdAsync(int userId);
        Task<int> DeleteExpiredTokensAsync();
    }
}
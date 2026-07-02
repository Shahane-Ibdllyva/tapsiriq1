using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class PasswordResetTokenRepository : Repository<PasswordResetToken>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(AppDbContext context) : base(context)
        {
        }

        
        public async Task<PasswordResetToken?> GetByTokenAsync(string token)
        {
            return await _context.PasswordResetTokens
                .Include(t => t.User) 
                .FirstOrDefaultAsync(t => t.Token == token
                                          && !t.IsUsed
                                          && !t.IsRevoked);
        }

        public async Task<PasswordResetToken?> GetLastValidTokenByUserIdAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _context.PasswordResetTokens
                .Where(t => t.UserId == userId
                            && !t.IsUsed
                            && !t.IsRevoked
                            && t.ExpiryDate > now)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        
        public async Task<int> DeleteExpiredTokensAsync()
        {
            var now = DateTime.UtcNow;
            var expiredTokens = _context.PasswordResetTokens
                .Where(t => t.ExpiryDate < now || t.IsUsed || t.IsRevoked);

            _context.PasswordResetTokens.RemoveRange(expiredTokens);
            return await _context.SaveChangesAsync();
        }
    }
}
using Horta.Domain.Model;
using Horta_Api.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Horta_Api.Infrastructure.Repositories
{
    public class UserResetPasswordCodeRepository : IUserResetPasswordCodeRepository
    {
            private readonly ConnectionContext _context = new ConnectionContext();

            public async Task<UserResetPasswordCode> CreateAsync(UserResetPasswordCode resetPasswordCode)
            {
                _context.UserResetPasswordCode.Add(resetPasswordCode);
                await _context.SaveChangesAsync();
                return resetPasswordCode;
            }

            public async Task<UserResetPasswordCode?> GetByEmailAndCodeAsync(string email, int code)
            {
                return await _context.UserResetPasswordCode
                    .FirstOrDefaultAsync(x => x.Email == email &&
                                             x.Code == code &&
                                             !x.IsUsed &&
                                             x.ExpiresAt > DateTime.UtcNow);
            }

            public async Task<UserResetPasswordCode?> GetActiveByEmailAsync(string email)
            {
                return await _context.UserResetPasswordCode
                    .Where(x => x.Email == email && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();
            }

            public async Task UpdateAsync(UserResetPasswordCode resetPasswordCode)
            {
                _context.UserResetPasswordCode.Update(resetPasswordCode);
                await _context.SaveChangesAsync();
            }

            public async Task<bool> ExistsActiveCodeAsync(string email)
            {
                return await _context.UserResetPasswordCode
                    .AnyAsync(x => x.Email == email &&
                                  !x.IsUsed &&
                                  x.ExpiresAt > DateTime.UtcNow);
            }
        }
    }



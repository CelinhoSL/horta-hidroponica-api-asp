using Horta.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Horta_Api.Infrastructure.Repositories
{
    public class UserVerificationCodeRepository : IUserVerificationCodeRepository
    {

        private readonly ConnectionContext _context = new ConnectionContext();

        public async Task<UserVerificationCode> CreateAsync(UserVerificationCode verificationCode)
        {
            _context.UserVerificationCode.Add(verificationCode);
            await _context.SaveChangesAsync();
            return verificationCode;
        }

        public async Task<UserVerificationCode?> GetByEmailAndCodeAsync(string email, int code)
        {
            return await _context.UserVerificationCode
                .FirstOrDefaultAsync(x => x.Email == email &&
                                         x.Code == code &&
                                         !x.IsUsed &&
                                         x.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<UserVerificationCode?> GetActiveByEmailAsync(string email)
        {
            return await _context.UserVerificationCode
                .Where(x => x.Email == email && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(UserVerificationCode verificationCode)
        {
            _context.UserVerificationCode.Update(verificationCode);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsActiveCodeAsync(string email)
        {
            return await _context.UserVerificationCode
                .AnyAsync(x => x.Email == email &&
                              !x.IsUsed &&
                              x.ExpiresAt > DateTime.UtcNow);
        }
    }
}
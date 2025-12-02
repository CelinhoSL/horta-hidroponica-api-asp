using Horta.Domain.Model;
using Horta_Api.Domain.Model;

namespace Horta_Api.Infrastructure.Repositories
{
    public interface IUserResetPasswordCodeRepository
    {
        Task<UserResetPasswordCode> CreateAsync(UserResetPasswordCode resetPasswordCode);
        Task<UserResetPasswordCode?> GetByEmailAndCodeAsync(string email, int code);
        Task<UserResetPasswordCode?> GetActiveByEmailAsync(string email);
        Task UpdateAsync(UserResetPasswordCode verificationCode);
        Task<bool> ExistsActiveCodeAsync(string email);
    }
}

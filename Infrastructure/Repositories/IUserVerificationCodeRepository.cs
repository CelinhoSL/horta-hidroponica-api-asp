using Horta.Domain.Model;

namespace Horta_Api.Infrastructure.Repositories
{
    public interface IUserVerificationCodeRepository
    {
        Task<UserVerificationCode> CreateAsync(UserVerificationCode verificationCode);
        Task<UserVerificationCode?> GetByEmailAndCodeAsync(string email, int code);
        Task<UserVerificationCode?> GetActiveByEmailAsync(string email);
        Task UpdateAsync(UserVerificationCode verificationCode);
        Task<bool> ExistsActiveCodeAsync(string email);
    }
}
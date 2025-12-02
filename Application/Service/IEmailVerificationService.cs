namespace Horta_Api.Application.Service
{
    public interface IEmailVerificationService
    {
        Task<bool> SendVerificationCodeAsync(string email);
        Task<bool> ValidateCodeAsync(string email, int code);
    }
}
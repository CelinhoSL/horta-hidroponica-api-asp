namespace Horta_Api.Application.Service
{
    public interface IEmailResetPasswordCodeService
    {
        Task<bool> SendVerificationCodeAsync(string email);
        Task<bool> ValidateCodeAsync(string email, int code);
    }
}

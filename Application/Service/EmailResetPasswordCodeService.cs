using Horta.Application.Service;
using Horta.Domain.Model;
using Horta_Api.Aplication.Service;
using Horta_Api.Aplication.Service.Validators;
using Horta_Api.Domain.Model;
using Horta_Api.Infrastructure.Repositories;

namespace Horta_Api.Application.Service
{
    public class EmailResetPasswordCodeService : IEmailResetPasswordCodeService
    {
        private readonly IUserResetPasswordCodeRepository _repository;
        private readonly IEmailService _emailService;

        public EmailResetPasswordCodeService(IUserResetPasswordCodeRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public async Task<bool> SendVerificationCodeAsync(string email)
        {
            // Validação do email
            EmailValidator.Validate(email);

            // Verificar se já existe um código ativo
            if (await _repository.ExistsActiveCodeAsync(email))
            {
                throw new InvalidOperationException("Já existe um código de verificação ativo para este email.");
            }

            // Gerar código seguro
            GenerateUserCode generator = new GenerateUserCode(email);
            var code = generator.Code;

            // Criar entidade
            var userResetPasswordCode = UserResetPasswordCode.Create(email, code);
            await _repository.CreateAsync(userResetPasswordCode);

            // Enviar email
            var subject = "Código de Segurança - Horta API";
            var body = $@"
                <h2>Código de Segurança</h2>
                <p>Olá!</p>
                <p>Seu código de segurança é: <strong>{code}</strong></p>
                <p>Este código expira em 3 minutos.</p>
                <p><em>Se você não solicitou este código, ignore este email.</em></p>
            ";

            await _emailService.SendEmailAsync(email, subject, body);
            return true;
        }

        public async Task<bool> ValidateCodeAsync(string email, int code)
        {
            EmailValidator.Validate(email);

            var userResetPasswordCode = await _repository.GetByEmailAndCodeAsync(email, code);

            if (userResetPasswordCode == null)
            {
                return false;
            }

            // Marcar como usado
            userResetPasswordCode.MarkAsUsed();

            await _repository.UpdateAsync(userResetPasswordCode);

            return true;
        }
    }
}
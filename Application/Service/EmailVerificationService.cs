using Horta.Application.Service;
using Horta.Domain.Model;
using Horta_Api.Aplication.Service;
using Horta_Api.Aplication.Service.Validators;
using Horta_Api.Application.Service;
using Horta_Api.Application.Services;
using Horta_Api.Infrastructure.Repositories;

namespace Horta_Api.Application.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IUserVerificationCodeRepository _repository;
        private readonly IEmailService _emailService;

        public EmailVerificationService(
            IUserVerificationCodeRepository repository,
            IEmailService emailService)
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
            var verificationCode = UserVerificationCode.Create(email, code);

            // Salvar no banco
            await _repository.CreateAsync(verificationCode);

            // Enviar email
            var subject = "Código de Verificação - Horta API";
            var body = $@"
                <h2>Código de Verificação</h2>
                <p>Olá!</p>
                <p>Seu código de verificação é: <strong>{code}</strong></p>
                <p>Este código expira em 3 minutos.</p>
                <p><em>Se você não solicitou este código, ignore este email.</em></p>
            ";

            await _emailService.SendEmailAsync(email, subject, body);

            return true;
        }

        public async Task<bool> ValidateCodeAsync(string email, int code)
        {
            EmailValidator.Validate(email);

            var verificationCode = await _repository.GetByEmailAndCodeAsync(email, code);

            if (verificationCode == null)
            {
                return false;
            }

            // Marcar como usado
            verificationCode.MarkAsUsed();
            await _repository.UpdateAsync(verificationCode);

            return true;
        }
    }
}
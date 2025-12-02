using Horta.Application.Service;
using Horta.Domain.Model;
using Horta_Api.Aplication.Service;
using Horta_Api.Application.Interfaces;
using Horta_Api.Application.Service;
using Horta_Api.Application.Services;

namespace Horta_Api.Application.Services
{
    public class EmailLogService : IEmailLogService
    {
        private readonly IUserService _userService;
        private readonly IUserLogService _userLogService;
        private readonly IEmailService _emailService;

        public EmailLogService(
            IUserService userService,
            IUserLogService userLogService,
            IEmailService emailService)
        {
            _userService = userService;
            _userLogService = userLogService;
            _emailService = emailService;
        }

        public async Task<bool> SendUserLogEmailAsync(int userId, int logId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            
            var userLog = await _userLogService.GetByIdAsync(logId);

            
            if (userLog.UserId != userId)
            {
                throw new InvalidOperationException("Log não pertence ao usuário especificado.");
            }

            
            var subject = $"Relatório de Log - {user.Username}";
            var body = BuildLogEmailBody(user, userLog);

            
            await _emailService.SendEmailAsync(user.Email, subject, body);

            return true;
        }

        public async Task<bool> SendUserLogEmailAsync(int userId, int logId, string customEmail)
        {
            
            var user = await _userService.GetUserByIdAsync(userId);

            
            var userLog = await _userLogService.GetByIdAsync(logId);

            
            if (userLog.UserId != userId)
            {
                throw new InvalidOperationException("Log não pertence ao usuário especificado.");
            }

            
            var subject = $"Relatório de Log - {user.Username}";
            var body = BuildLogEmailBody(user, userLog);

            
            await _emailService.SendEmailAsync(customEmail, subject, body);

            return true;
        }

        private string BuildLogEmailBody(User user, UserLog userLog)
        {
            return $@"
                <h2>Relatório de Log do Usuário</h2>
                <h3>Informações do Usuário:</h3>
                <p><strong>Nome:</strong> {user.Username}</p>
                <p><strong>Email:</strong> {user.Email}</p>
                
                
                <h3>Informações do Log:</h3>
                <p><strong>Data/Hora:</strong> {userLog.CreatedAt:dd/MM/yyyy HH:mm:ss}</p>
                <p><strong>User Agent:</strong> {userLog.UserAgent}</p>
                <p><strong>IP:</strong> {userLog.IpAddress}</p>
                
                <hr>
                <p><em>Este é um email automático do sistema Horta API.</em></p>
            ";
        }

        
        public async Task<bool> SendUserLogsReportAsync(int userId)
        {
            
            var user = await _userService.GetUserByIdAsync(userId);

            

            var subject = $"Relatório Completo de Logs - {user.Username}";
            var body = $@"
                <h2>Relatório Completo de Logs</h2>
                <h3>Usuário: {user.Username}</h3>
                
                
                <p>Para ver os logs detalhados, acesse o sistema.</p>
                
                <hr>
                <p><em>Este é um email automático do sistema Horta API.</em></p>
            ";

            await _emailService.SendEmailAsync(user.Email, subject, body);
            return true;

        }
        public async Task<bool> SendSecurityAlertEmailAsync(int userId, int logId, string registeredIp, string currentIp)
        {
            // Buscar dados do usuário
            var user = await _userService.GetUserByIdAsync(userId);

            // Buscar dados do log
            var userLog = await _userLogService.GetByIdAsync(logId);

            // Montar email de alerta de segurança
            var subject = "🚨 ALERTA DE SEGURANÇA - Login detectado com IP diferente";
            var body = BuildSecurityAlertEmailBody(user, userLog, registeredIp, currentIp);

            // Enviar email oi
            await _emailService.SendEmailAsync(user.Email, subject, body);

            return true;
        }

        private string BuildSecurityAlertEmailBody(User user, UserLog userLog, string registeredIp, string currentIp)
        {
            return $@"
        <h1>ALERTA DE SEGURANÇA</h1>
        <p>Olá, {user.Username}!</p>

        <p>Detectamos um login na sua conta com um endereço IP diferente do habitual.</p>

        <h2>Detalhes do Login:</h2>
        <p>Data/Hora: {userLog.CreatedAt:dd/MM/yyyy HH:mm:ss}</p>
        <p>IP Cadastrado: {registeredIp}</p>
        <p>IP do Login: {currentIp}</p>
        <p>Navegador: {userLog.UserAgent}</p>

        <h2>O que fazer?</h2>
        <p>Se foi você: nenhuma ação é necessária. Seu IP pode ter mudado naturalmente.</p>
        <p>Se não foi você:</p>
        <ul>
            <li>Altere sua senha imediatamente</li>
            <li>Verifique se há atividades suspeitas na sua conta</li>
            <li>Entre em contato conosco se precisar de ajuda</li>
        </ul>

        <hr>

        <p>Este é um email automático do sistema Horta API para sua segurança.</p>
        <p>Se você recebeu este email por engano, pode ignorá-lo com segurança.</p>
    ";
        }
    }
}
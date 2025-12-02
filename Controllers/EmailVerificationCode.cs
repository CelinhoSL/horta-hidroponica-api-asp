
using Horta_Api.Application.Interfaces;
using Horta_Api.Application.Service;
using Horta_Api.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Horta_Api.Controllers
{
    [ApiController]
    [Route("api/email-verification")]
    public class EmailVerificationController : ControllerBase
    {
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IUserService _userService;
        private readonly ILogger<EmailVerificationController> _logger;

        public EmailVerificationController(IEmailVerificationService emailVerificationService, IUserService userService, ILogger<EmailVerificationController> logger)
        {
            _emailVerificationService = emailVerificationService;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromQuery] SendVerificationCodeDTO emailDTO)
        {
            emailDTO.Validate();

            if (string.IsNullOrWhiteSpace(emailDTO.Email))
                return BadRequest("Por favor, forneça o e-mail do destinatário.");

            try
            {
                _logger.LogInformation("Sending verification code to {Email}", emailDTO.Email); 
                if (await _userService.ExistsEmail(emailDTO.Email))
                {
                    await _emailVerificationService.SendVerificationCodeAsync(emailDTO.Email);
                    return Ok($"Código de verificação enviado com sucesso para {emailDTO.Email}.");
                }
                else
                {
                    return BadRequest("O usuário já existe");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending verification code to {Email}", emailDTO.Email); 
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("validate-code")]
        public async Task<IActionResult> ValidateCode([FromBody] CreateUserDto dto, [FromQuery] VerificationCodeDTO dtoCode)
        {
            dto.Validate();
            dtoCode.Validate();

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Por favor, forneça o e-mail.");

            dto.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            dto.UserAgent = Request.Headers["User-Agent"].ToString();

            try
            {
                var isValid = await _emailVerificationService.ValidateCodeAsync(dto.Email, dtoCode.Code);
                if (!isValid)
                    return BadRequest("Código inválido, expirado ou já utilizado.");

                var user = await _userService.CreateUserAsync(dto);
                var token = TokenService.GenerateToken(user);

                _logger.LogInformation("Usuário criado com sucesso{UserId}", user.UserId);
                return Ok(
                    new
                    {
                        Auth = new
                        {
                            Token = token
                        },
                        User = new
                        {
                            UserId = user.UserId,
                            Username = user.Username,
                            Email = dto.Email,
                            UpdatedAt = user.UpdatedAt
                        },
                        Session = new
                        {
                            IpAddress = user.IpAddress,
                            UserAgent = user.UserAgent
                        }
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
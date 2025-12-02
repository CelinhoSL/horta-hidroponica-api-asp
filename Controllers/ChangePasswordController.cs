using Horta_Api.Application.Service;
using Horta_Api.Domain.DTOs;
using Horta_Api.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

namespace Horta_Api.Controllers
{
    
    public class ChangePasswordController : ControllerBase
    {
        private readonly IEmailResetPasswordCodeService _emailResetPasswordService;
        private readonly IUpdateUserService _updateUserService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserService _userService;
        private readonly ILogger<ChangePasswordController> _logger;

        public ChangePasswordController(IEmailResetPasswordCodeService emailResetPasswordCodeService, IUpdateUserService updateUserService,
            IPasswordHasher passwordHasher, IUserService userService, ILogger<ChangePasswordController> logger)
        {
            _emailResetPasswordService = emailResetPasswordCodeService;
            _updateUserService = updateUserService;
            _passwordHasher = passwordHasher;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("api/users/send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromQuery] SendResetCodeDTO resetCodeDTO)
        {
            resetCodeDTO.Validate();

            if (string.IsNullOrWhiteSpace(resetCodeDTO.Email))
                return BadRequest("Por favor, forneça o e-mail do destinatário.");

            try
            {
                if (await _userService.ExistsEmail(resetCodeDTO.Email) == false)
                {
                    await _emailResetPasswordService.SendVerificationCodeAsync(resetCodeDTO.Email);

                    _logger.LogInformation("Código de redefinição de senha enviao paro email: {Email}", resetCodeDTO.Email);

                    return Ok($"Código de verificação enviado com sucesso para {resetCodeDTO.Email}.");
                    
                }
                else
                {
                    return BadRequest("O usuário já existe");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPut("api/users/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO, [FromQuery] VerificationCodeDTO dtoCode)
        {
            changePasswordDTO.Validate();
            dtoCode.Validate();
            
            try
            {
                var user = await _userService.GetByEmailAsync(changePasswordDTO.Email);
                if (user == null)
                {
                    return NotFound(new { error = "Usuário não encontrado" });
                }
                
                var userId = user.UserId;

                var isCodeValid = await _emailResetPasswordService.ValidateCodeAsync(changePasswordDTO.Email, dtoCode.Code);
                if (!isCodeValid)
                {
                    return BadRequest(new { error = "Código de verificação inválido ou expirado." });
                }
                user.Password = _passwordHasher.HashPassword(changePasswordDTO.NewPassword);
                var updatedUser = await _updateUserService.UpdateUserWithoutPasswordAsync(userId, user.Password);
                return Ok(new
                {
                    message = "Senha alterada com sucesso",
                    user = new
                    {
                        id = updatedUser.UserId,
                        username = updatedUser.Username,
                        email = updatedUser.Email
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Código inválido, não corresponde ao iniviado ao email: {Email}", changePasswordDTO.Email);
                return BadRequest(new { error = ex.Message });

            }
        }

    }
}
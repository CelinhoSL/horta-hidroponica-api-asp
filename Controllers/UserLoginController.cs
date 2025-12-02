using Horta.Domain.Model;
using Horta_Api.Application.Interfaces;
using Horta_Api.Application.Service;
using Horta_Api.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Horta_Api.Application.Services;

namespace Horta_Api.Controllers
{
    [ApiController]
    [Route("api/users/")]
    public class UserLoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserLogService _userLogService;
        private readonly IEmailLogService _emailLogService;
        private readonly ILogger<UserLoginController> _logger;

        public UserLoginController(
            IUserService userService,
            IUserLogService userLogService,
            IEmailLogService emailLogService,
            ILogger<UserLoginController> logger )
        {
            _userService = userService;
            _userLogService = userLogService;
            _emailLogService = emailLogService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Tentativa de login: {Email}", loginDto.Email);
                var currentIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var currentUserAgent = Request.Headers["User-Agent"].ToString();

                loginDto.IpAddress = currentIp;
                loginDto.UserAgent = currentUserAgent;

                
                var result = await _userService.LoginAsync(loginDto);

                _logger.LogInformation("Login bem-sucedido: UserId {UserId}", result.UserId);
                
                await CheckAndHandleSecurityAlert(result.UserId, currentIp, currentUserAgent);

                var user = new User
                {
                    UserId = result.UserId,
                    Username = result.Username,
                    IpAddress = currentIp,
                    UserAgent = currentUserAgent,
                    UpdatedAt = DateTime.UtcNow,
                };
                var token = TokenService.GenerateToken(user);
                return Ok(new
                {
                    Auth = new
                    {
                        Token = token
                    },
                    User = new
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = loginDto.Email,
                        UpdatedAt = user.UpdatedAt
                    },
                    Session = new
                    {
                        IpAddress = user.IpAddress,
                        UserAgent = user.UserAgent
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Falha no login: {Email} - {Error}", loginDto.Email, ex.Message);
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no login: {Email}", loginDto.Email);
                return StatusCode(500, new { message = "Erro interno no servidor.", detail = ex.Message });
            }
        }

        private async Task CheckAndHandleSecurityAlert(int userId, string currentIp, string currentUserAgent)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                bool isDifferentIp = !string.IsNullOrEmpty(user.IpAddress) &&
                                   user.IpAddress != currentIp;
                if (isDifferentIp)
                {
                    var securityLog = new UserLog
                    {
                        UserId = userId,
                        IpAddress = currentIp,
                        UserAgent = currentUserAgent,
                        CreatedAt = DateTime.UtcNow,
                        
                    };

                    var createdLog = await _userLogService.CreateAsync(securityLog);

                    await SendSecurityAlertEmail(userId, createdLog.Id, user.IpAddress, currentIp);
                }

                else
                {
                    var normalLog = new UserLog
                    {
                        UserId = userId,
                        IpAddress = currentIp,
                        UserAgent = currentUserAgent,
                        CreatedAt = DateTime.UtcNow,
                        
                    };

                    await _userLogService.CreateAsync(normalLog);
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Erro ao processar alerta de segurança: {ex.Message}");
                
            }
        }

        private async Task SendSecurityAlertEmail(int userId, int logId, string registeredIp, string currentIp)
        {
            try
            {
                await _emailLogService.SendSecurityAlertEmailAsync(userId, logId, registeredIp, currentIp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar email de alerta: {ex.Message}");
            }
        }
    }
}
using Horta_Api.Application.Service;
using Horta_Api.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;
using Horta_Api.Infrastructure.Security;

namespace Horta_Api.Controllers
{
    public class UpdateUserController : ControllerBase
    {
        private readonly IUpdateUserService _updateUserService;
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UpdateUserController> _logger;
        public UpdateUserController(IUpdateUserService updateUserService, IUserService userService, IPasswordHasher passwordHasher, ILogger<UpdateUserController> logger)
        {
            _updateUserService = updateUserService;
            _userService = userService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }
        
        [Authorize]
        [HttpPut("api/users/update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO updateUserDTO)
        {
            try
            {
                var userIdString = User.FindFirst("id")?.Value;

                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                {
                    return Unauthorized(new { error = "Token inválido ou ID não encontrado" });
                }

                var user = await _userService.GetUserByIdAsync(userId);
                var currentHashPassword = user.Password;

                if (user == null)
                {
                    return NotFound(new { error = "Usuário não encontrado" });
                }
                updateUserDTO.Validate();

                if (!string.IsNullOrWhiteSpace(updateUserDTO.Username) &&
                    updateUserDTO.Username != user.Username)
                {
                    user.Username = updateUserDTO.Username;
                }

                if (!string.IsNullOrWhiteSpace(updateUserDTO.Email) &&
                    updateUserDTO.Email != user.Email)
                {
                    user.Email = updateUserDTO.Email;
                }

                if (!string.IsNullOrWhiteSpace(updateUserDTO.Password))
                {
                    user.Password = _passwordHasher.HashPassword(updateUserDTO.Password);
                }

                var updatedUser = await _updateUserService.UpdateUserAsync(
                    updateUserDTO.CurrentPassword,
                    user, currentHashPassword
                );

                _logger.LogInformation("User update requested: UserId {UserId}", userId);
                return Ok(new
                {
                    message = "Usuário atualizado com sucesso",
                    user = new
                    {
                        
                        username = updatedUser.Username,
                        email = updatedUser.Email
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                var innerEx = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { error = "Erro interno do servidor", details = innerEx });
            }
        }
    }
}
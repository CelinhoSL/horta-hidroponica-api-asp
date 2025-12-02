using Horta_Api.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Horta_Api.Controllers
{
    public class DeleteUserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<DeleteUserController> _logger;

        public DeleteUserController(IUserService userService, ILogger<DeleteUserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        [HttpDelete("api/users/delete")]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {
            try
            {
                var userIdstring = User.FindFirst("id")?.Value;
                var userId = int.Parse(userIdstring);
                await _userService.DeleteUserAsync(userId);
                _logger.LogInformation("Um usuário foi deletado");
                return Ok(new { message = "Usuário deletado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

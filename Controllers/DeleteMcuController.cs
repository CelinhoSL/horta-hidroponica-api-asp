using Horta_Api.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Horta_Api.Controllers
{
    public class DeleteMcuController : ControllerBase
    {
        private readonly IMainMcuConfigService _mainMcuConfigService;
        private readonly ILogger<DeleteMcuController> _logger;

        public DeleteMcuController(IMainMcuConfigService mainMcuConfigService, ILogger<DeleteMcuController> logger)
        {
            _mainMcuConfigService = mainMcuConfigService;
            _logger = logger;
        }


        [HttpDelete("api/mcu/delete")]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {
            try
            {
                var userIdstring = User.FindFirst("id")?.Value;
                var userId = int.Parse(userIdstring);
                var mcuId = await _mainMcuConfigService.GetMcuIdByUserId(userId);

                await _mainMcuConfigService.DeleteMcuAsync(mcuId);
                _logger.LogInformation("MCU deletado com sucesso para UserId {UserId}", userId);
                return Ok(new { message = "Mcu deletado com sucesso" });
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

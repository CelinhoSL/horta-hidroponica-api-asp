using Microsoft.AspNetCore.Mvc;
using Horta_Api.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Horta_Api.Application.Service; 

namespace Horta_Api.Controllers
{
    [ApiController]
    public class MainMcuController : ControllerBase
    {
        private readonly IMainMcuService _service; 
        private readonly IMainMcuConfigService _mainMcuConfigService;
        public MainMcuController(IMainMcuService service, IMainMcuConfigService mainMcuConfigService) 
        {
            _service = service;
            _mainMcuConfigService = mainMcuConfigService;
        }
        [Authorize]
        [HttpGet("api/mcus/{id}/light")]
        public async Task<IActionResult> GetLightStatus()
        {
            var userIdstring = User.FindFirst("id")?.Value;
            var userId = int.Parse(userIdstring);
            var mcuId = await _mainMcuConfigService.GetMcuIdByUserId(userId);
            var status = await _service.GetLightStatusAsync(mcuId);
            if (status == null) return NotFound();
            return Ok(status);
        }
        [Authorize]
        [HttpPut("api/mcus/{id}/light")]
        public async Task<IActionResult> UpdateLightStatus([FromBody] bool status)
        {
            var userIdstring = User.FindFirst("id")?.Value;
            var userId = int.Parse(userIdstring);
            var mcuId = await _mainMcuConfigService.GetMcuIdByUserId(userId);
            var ok = await _service.UpdateLightStatus(mcuId, status);
            return ok ? Ok() : BadRequest();
        }
        [Authorize]
        [HttpGet("api/mcus/{id}/pump")]
        public async Task<IActionResult> GetPumpStatus()
        {
            var userIdstring = User.FindFirst("id")?.Value;
            var userId = int.Parse(userIdstring);
            var mcuId = await _mainMcuConfigService.GetMcuIdByUserId(userId);
            var status = await _service.GetPumpRelayStatusAsync(mcuId);
            if (status == null) return NotFound();
            return Ok(status);
        }
        [Authorize]
        [HttpPut("api/mcus/{id}/pump")]
        public async Task<IActionResult> UpdatePumpStatus([FromBody] bool status)
        {
            var userIdstring = User.FindFirst("id")?.Value;
            var userId = int.Parse(userIdstring);
            var mcuId = await _mainMcuConfigService.GetMcuIdByUserId(userId);
            var ok = await _service.UpdatePumpRelayStatus(mcuId, status);
            return ok ? Ok() : BadRequest();
        }
    }
}
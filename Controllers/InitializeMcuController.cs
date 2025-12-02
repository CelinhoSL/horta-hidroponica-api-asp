using Horta.Domain.Model;
using Horta_Api.Application.Interfaces;
using Horta_Api.Application.Service;
using Horta_Api.Application.Services;
using Horta_Api.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Horta_Api.Controllers
{
    public class InitializeMcuController : ControllerBase
    {
        private readonly IMainMcuConfigService _mainMcuConfigService;
        private readonly IWaterLevelSensorService _waterLevelSensorService;
        private readonly ILightSensorService _lightSensorService;
        private readonly ITemperatureSensorService _temperatureSensorService;
        private readonly ICameraService _cameraService;
        private readonly ILogger<InitializeMcuController> _logger;

        public InitializeMcuController(IMainMcuConfigService mainMcuConfigService, ILightSensorService lightSensorService, IWaterLevelSensorService waterLevelSensorService, ITemperatureSensorService temperatureSensorService,ICameraService cameraService, ILogger<InitializeMcuController> logger)
        {
            _mainMcuConfigService = mainMcuConfigService;
            _lightSensorService = lightSensorService;
            _waterLevelSensorService = waterLevelSensorService;
            _temperatureSensorService = temperatureSensorService;
            _cameraService = cameraService;
            _logger = logger;
        }
        //
        [Authorize]
        [HttpPost("api/mcu/initialize")]
        public async Task<IActionResult> InitializeMcu([FromBody] InitializeMainControllerDTO mcuController)
        {
            try
            {
                var userId = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {

                    return BadRequest(new { error = "User ID não encontrado no token" });
                }

                mcuController.UserId = int.Parse(userId);
                _logger.LogInformation("MCU inicializada pelo usuário {UserId}", userId);
                var mainController = await _mainMcuConfigService.CreateAsync(mcuController);


                var waterLevelSensor = await _waterLevelSensorService.CreateWaterLevelSensorAsync(mainController.Id);
                var lightSensor = await _lightSensorService.CreateLightSensorAsync(mainController.Id);
                var temperatureSensor = await _temperatureSensorService.CreateTemperatureSensorAsync(mainController.Id);
                var camera =  await _cameraService.CreateCameraAsync(mainController.Id);

                return Ok(new
                {
                    MainControllerId = mainController.Id,
                    LightSensorId = lightSensor?.Id,
                    WaterLevelSensorId = waterLevelSensor?.Id,
                    TemperatureSensorId = temperatureSensor?.Id,
                    CameraId = camera?.Id
                });
            }
            catch (Exception ex)
            {
                var innerEx = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "Falha ao iniciar a MCU {UserId} {UserId}");

                return BadRequest(new { error = innerEx });
            }
        }

        [HttpPost("api/mcu/login")]
        public async Task<IActionResult> McuLogin([FromBody] McuLoginDTO loginDto)
        {
            try
            {

                loginDto.Validate();

                var mainController = await _mainMcuConfigService.LoginAsync(loginDto.IpAddress, loginDto.DeviceSecretKey);
                var user = new User
                {
                    UserId = mainController.UserId
                };
                var token = TokenService.GenerateToken(user);

                var waterLevelSensor = await _waterLevelSensorService.GetWaterLevelSensorAsync(mainController.Id);
                var lightSensor = await _lightSensorService.GetLightSensorAsync(mainController.Id);
                var temperatureSensor = await _temperatureSensorService.GetTemperatureSensorAsync(mainController.Id);
                var camera = await _cameraService.GetCameraIdAsync(mainController.Id);

                _logger.LogInformation("Login realizado na MCU {MCU IP}", loginDto.IpAddress);
                return Ok(new
                {
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Erro de credências {MCU IP}", loginDto.IpAddress);
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {

                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor.", detail = ex.Message });
            }
        }


    }
}

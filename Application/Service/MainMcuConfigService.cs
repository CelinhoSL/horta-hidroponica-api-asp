using Horta.Domain.Model;
using Horta_Api.Domain.DTOs;
using Horta_Api.Infrastructure.Repositories;
using Horta_Api.Domain;
using Horta_Api.Infrastructure.Security;

namespace Horta_Api.Application.Service
{
    public class MainMcuConfigService : IMainMcuConfigService
    {
        private readonly IMainMcuConfigRepository _mainMcuConfigRepository;
        private readonly IPasswordHasher _passwordHasher;

        public MainMcuConfigService(IMainMcuConfigRepository mainMcuConfigRepository, IPasswordHasher passwordHasher)
        {
            _mainMcuConfigRepository = mainMcuConfigRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<MainController> CreateAsync(InitializeMainControllerDTO mcuController)
        {
            mcuController.Validate();

            var mainController = new MainController
            {
                UserId = mcuController.UserId,
                IpAddress = mcuController.ipAddress,
                DeviceSecreteKey = _passwordHasher.HashPassword(mcuController.DeviceSecretKey)
            };

            return await _mainMcuConfigRepository.CreateAsync(mainController);
        }

        public async Task<MainController> GetMcuByIp(string ip)
        {
            return await _mainMcuConfigRepository.GetMcuByIp(ip);
        }

        public async Task<MainController> LoginAsync(string ipAddress, string deviceSecretKey)
        {
            var mainController = await _mainMcuConfigRepository.GetMcuByIp(ipAddress);
            if (mainController == null)
                throw new InvalidOperationException("MCU não encontrado");
            if (!_passwordHasher.VerifyPassword(deviceSecretKey, mainController.DeviceSecreteKey))
                throw new UnauthorizedAccessException("Credenciais inválidas");
            return mainController;
        }

        public async Task<MainController> DeleteMcuAsync(int id)
        {
           return await _mainMcuConfigRepository.DeleteMcuAsync(id);
        }

        public async Task<int> GetMcuIdByUserId(int userId)
        {
            return await _mainMcuConfigRepository.GetMcuIdByUserId(userId);
        }
    }
}

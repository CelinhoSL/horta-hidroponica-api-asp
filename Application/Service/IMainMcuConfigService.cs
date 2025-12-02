using Horta.Domain.Model;
using Horta_Api.Domain.DTOs;

namespace Horta_Api.Application.Service
{
    public interface IMainMcuConfigService
    {
        public Task<MainController> CreateAsync(InitializeMainControllerDTO controller);
        Task<int> GetMcuIdByUserId(int userId);
        Task<MainController> GetMcuByIp(string ip);
        Task<MainController> LoginAsync(string ipAddress, string deviceSecretKey);
        Task<MainController> DeleteMcuAsync(int id);
    }
}

using Horta.Domain.Model;
using Horta_Api.Domain.Model;
namespace Horta_Api.Application.Services
{
    public interface ITemperatureSensorService
    {
        Task<float?> GetTemperatureValueAsync(int id);
        Task<float?> UpdateTemperatureValueAsync(int mainControllerId, float value);
        Task<TemperatureSensor?> GetTemperatureSensorAsync(int id);
        Task<TemperatureSensor> CreateTemperatureSensorAsync(int mainControllerId);
        Task<int> GetTemperatureSensorIdByMCUIdAsync(int mainControllerId);
    }
}
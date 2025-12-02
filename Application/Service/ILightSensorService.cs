using Horta.Domain.Model;
using Horta_Api.Domain.Model;

namespace Horta_Api.Application.Services
{
    public interface ILightSensorService
    {
        Task<float?> GetLightValueAsync(int id);
        Task<float?> UpdateLightValueAsync(int mainControllerId, float value);
        Task<LightSensor?> GetLightSensorAsync(int id);
        Task<LightSensor> CreateLightSensorAsync(int mainControllerId);
        Task<int> GetLightSensorIdByMCUIdAsync(int mainControllerId);
    }
}
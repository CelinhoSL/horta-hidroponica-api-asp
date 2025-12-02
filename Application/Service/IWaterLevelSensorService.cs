using Horta.Domain.Model;

namespace Horta_Api.Application.Services
{
    public interface IWaterLevelSensorService
    {
        Task<float?> GetWaterLevelValueAsync(int id);
        Task<float?> UpdateWaterLevelValueAsync(int mainControllerId, float value);
        Task<WaterLevelSensor?> GetWaterLevelSensorAsync(int id);
        Task<WaterLevelSensor> CreateWaterLevelSensorAsync(int mainControllerId);
        Task<int> GetWaterLevelSensorByMCUIdAsync(int mainControllerId);
    }
}
using Horta.Domain.Model;
using Horta_Api.Domain.Repositories;
using Horta_Api.Infrastructure.Repositories;

namespace Horta_Api.Application.Services
{
    public class WaterLevelSensorService : IWaterLevelSensorService
    {
        
        private readonly ISensorRepository _sensorRepository;

        public WaterLevelSensorService(ISensorRepository sensorRepository)
        {
            _sensorRepository = sensorRepository;
        }

        public async Task<float?> GetWaterLevelValueAsync(int id)
        {
            return await _sensorRepository.GetValueAsync<WaterLevelSensor>(id);
        }

        public async Task<float?> UpdateWaterLevelValueAsync(int mainControllerId, float value)
        {
            return await _sensorRepository.SetValueAsync<WaterLevelSensor>(mainControllerId, value);
        }

        public async Task<WaterLevelSensor?> GetWaterLevelSensorAsync(int id)
        {
            return await _sensorRepository.GetSensorAsync<WaterLevelSensor>(id);
        }

        public async Task<WaterLevelSensor> CreateWaterLevelSensorAsync(int mainControllerId)
        {
            return await _sensorRepository.CreateSensorAsync<WaterLevelSensor>(mainControllerId);
        }

        public async Task<int> GetWaterLevelSensorByMCUIdAsync(int mainControllerId)
        {
            return await _sensorRepository.GetSensorIdAsync<WaterLevelSensor>(mainControllerId);
        }
    }
}
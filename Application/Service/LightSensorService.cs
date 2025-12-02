using Horta.Domain.Model;
using Horta_Api.Domain.Model;
using Horta_Api.Domain.Repositories;
using Horta_Api.Infrastructure.Repositories;

namespace Horta_Api.Application.Services
{
    public class LightSensorService : ILightSensorService
    {
        private readonly ISensorRepository _sensorRepository;

        public LightSensorService(ISensorRepository sensorRepository)
        {
            _sensorRepository = sensorRepository;
        }

        public async Task<float?> GetLightValueAsync(int id)
        {
            return await _sensorRepository.GetValueAsync<LightSensor>(id);
        }

        public async Task<float?> UpdateLightValueAsync(int mainControllerId, float value)
        {
            return await _sensorRepository.SetValueAsync<LightSensor>(mainControllerId, value);
        }

        public async Task<LightSensor?> GetLightSensorAsync(int id)
        {
            return await _sensorRepository.GetSensorAsync<LightSensor>(id);
        }

        public async Task<LightSensor> CreateLightSensorAsync(int mainControllerId)
        {
            return await _sensorRepository.CreateSensorAsync<LightSensor>(mainControllerId);
        }

        public async Task<int> GetLightSensorIdByMCUIdAsync(int mainControllerId)
        {
            return await _sensorRepository.GetSensorIdAsync<LightSensor>(mainControllerId);
        }
    }
}
using Horta.Domain.Model;
using Horta_Api.Domain.Model;
using Horta_Api.Domain.Repositories;
using Horta_Api.Infrastructure.Repositories;
namespace Horta_Api.Application.Services
{
    public class TemperatureSensorService : ITemperatureSensorService
    {
        private readonly ISensorRepository _sensorRepository;
        public TemperatureSensorService(ISensorRepository sensorRepository)
        {
            _sensorRepository = sensorRepository;
        }
        public async Task<float?> GetTemperatureValueAsync(int id)
        {
            return await _sensorRepository.GetValueAsync<TemperatureSensor>(id);
        }
        public async Task<float?> UpdateTemperatureValueAsync(int mainControllerId, float value)
        {
            return await _sensorRepository.SetValueAsync<TemperatureSensor>(mainControllerId, value);
        }
        public async Task<TemperatureSensor?> GetTemperatureSensorAsync(int id)
        {
            return await _sensorRepository.GetSensorAsync<TemperatureSensor>(id);
        }
        public async Task<TemperatureSensor> CreateTemperatureSensorAsync(int mainControllerId)
        {
            return await _sensorRepository.CreateSensorAsync<TemperatureSensor>(mainControllerId);
        }
        public async Task<int> GetTemperatureSensorIdByMCUIdAsync(int mainControllerId)
        {
            return await _sensorRepository.GetSensorIdAsync<TemperatureSensor>(mainControllerId);
        }
    }
}
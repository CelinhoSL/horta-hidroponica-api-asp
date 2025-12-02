using Horta.Domain.Model;

namespace Horta_Api.Domain.Repositories
{
    public interface ISensorRepository
    {
        Task<int> GetSensorIdAsync<TSensor>(int mainControllerId) where TSensor : Sensor;
        Task<float?> SetValueAsync<TSensor>(int mainControllerId, float value) where TSensor : Sensor;
        Task<float?> GetValueAsync<TSensor>(int id) where TSensor : Sensor;
        Task<TSensor?> GetSensorAsync<TSensor>(int id) where TSensor : Sensor;
        Task<TSensor> CreateSensorAsync<TSensor>(int mainControllerId) where TSensor : Sensor, new();
    }
}
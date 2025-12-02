using Horta.Domain.Model;
using Horta_Api.Domain.Model;
using Horta_Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Horta_Api.Infrastructure.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        protected readonly ConnectionContext _context; 

        public SensorRepository(ConnectionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> GetSensorIdAsync<TSensor>(int mainControllerId) where TSensor : Sensor
        {
            try
            {
                return await _context.Set<TSensor>()
                    .Where(s => s.MainControllerId == mainControllerId)
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sensor ID for MainController ID {mainControllerId}", ex);
            }
        }

        
        public async Task<float?> GetValueAsync<TSensor>(int id) where TSensor : Sensor
        {
            try
            {
                return await _context.Set<TSensor>()
                    .Where(s => s.Id == id)
                    .Select(s => s.Value)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving value of sensor type {typeof(TSensor).Name} with ID {id}", ex);
            }
        }

        
        public async Task<float?> SetValueAsync<TSensor>(int mainControllerId, float value) where TSensor : Sensor
        {
            try
            {
                var sensor = await _context.Set<TSensor>()
                    .FirstOrDefaultAsync(s => s.MainControllerId == mainControllerId);

                if (sensor == null)
                {
                    return null; 
                }

                sensor.Value = value;
                await _context.SaveChangesAsync();
                return sensor.Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating sensor value for MainController ID {mainControllerId}", ex);
            }
        }


       
        public async Task<TSensor?> GetSensorAsync<TSensor>(int id) where TSensor : Sensor
        {
            try
            {
                return await _context.Set<TSensor>()
                    .FirstOrDefaultAsync(s => s.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sensor of type {typeof(TSensor).Name} with ID {id}", ex);
            }
        }

        
        public async Task<TSensor> CreateSensorAsync<TSensor>(int mainControllerId) where TSensor : Sensor, new()
        {
            try
            {
                
                var mainControllerExists = await _context.Set<MainController>()
                    .AnyAsync(mc => mc.Id == mainControllerId);

                if (!mainControllerExists)
                {
                    throw new ArgumentException($"MainController com ID {mainControllerId} não existe");
                }

                
                var existingSensor = await _context.Set<TSensor>()
                    .FirstOrDefaultAsync(s => s.MainControllerId == mainControllerId);

                if (existingSensor != null)
                {
                    throw new InvalidOperationException($"Já existe um sensor do tipo {typeof(TSensor).Name} para o MainController ID {mainControllerId}");
                }

                var sensor = new TSensor
                {
                    MainControllerId = mainControllerId,
                    Value = 0,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Set<TSensor>().Add(sensor);
                await _context.SaveChangesAsync();

                
                if (sensor.Id <= 0)
                {
                    throw new InvalidOperationException($"Sensor do tipo {typeof(TSensor).Name} foi criado mas não possui ID válido");
                }

                return sensor;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar sensor do tipo {typeof(TSensor).Name} para MainController ID {mainControllerId}: {ex.Message}", ex);
            }
        }
    }
}
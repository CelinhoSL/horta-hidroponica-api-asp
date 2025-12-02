using Horta.Domain.Model;

namespace Horta_Api.Infrastructure.Repositories
{
    public interface IMainMcuRepository
    {
        public Task<bool> UpdateLightStatus(bool lightStatus, int controllerId);
        public Task<bool> UpdatePumpRelayStatus(bool pumpRelayStatus, int controllerId);
        public Task<bool?> GetLightStatusAsync(int controllerId);
        public Task<bool?> GetPumpRelayStatusAsync(int controllerId);
        
    }
}

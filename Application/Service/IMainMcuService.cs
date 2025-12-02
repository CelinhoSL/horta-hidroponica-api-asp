
using Horta.Domain.Model;

namespace Horta_Api.Application.Interfaces
{
    public interface IMainMcuService
    {
        Task<bool> UpdateLightStatus(int controllerId, bool lightStatus);
        Task<bool> UpdatePumpRelayStatus(int controllerId, bool pumpRelayStatus);
        Task<bool?> GetLightStatusAsync(int controllerId);
        Task<bool?> GetPumpRelayStatusAsync(int controllerId);
    }
}
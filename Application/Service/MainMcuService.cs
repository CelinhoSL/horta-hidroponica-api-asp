using Horta_Api.Application.Interfaces;
using Horta_Api.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Horta_Api.Infrastructure.Services
{
    public class MainMcuService : IMainMcuService
    {
        private readonly IMainMcuRepository _repo;
        private readonly IServiceProvider _serviceProvider;

        public MainMcuService(IMainMcuRepository repo, IServiceProvider serviceProvider)
        {
            _repo = repo;
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> UpdateLightStatus(int controllerId, bool lightStatus)
        {
            var newLightStatus = await _repo.UpdateLightStatus(lightStatus, controllerId);
            
            return newLightStatus;
        }

        public async Task<bool> UpdatePumpRelayStatus(int controllerId, bool pumpRelayStatus)
        {
            var newPumpRelayStatus = await _repo.UpdatePumpRelayStatus(pumpRelayStatus, controllerId);
            
            return newPumpRelayStatus;
        }

        public Task<bool?> GetLightStatusAsync(int controllerId)
        {
            return _repo.GetLightStatusAsync(controllerId);
        }

        public Task<bool?> GetPumpRelayStatusAsync(int controllerId)
        {
            return _repo.GetPumpRelayStatusAsync(controllerId);
        }
    }
}

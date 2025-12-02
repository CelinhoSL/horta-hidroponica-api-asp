using Horta.Domain.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Horta_Api.Infrastructure.Repositories
{
    public class MainMcuRepository : IMainMcuRepository
    {
        private readonly ConnectionContext _context = new ConnectionContext();


        public async Task<bool> UpdateLightStatus(bool lightStatus, int controllerId)
        {
            try
            {
                await _context.MainController
                    .Where(c => c.Id == controllerId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.LightStatus, lightStatus));

                return true; 
            }
            catch
            {
                return false;
            }

        }
         
        public async Task<bool> UpdatePumpRelayStatus(bool pumpRelayStatus, int controllerId)
        {
            try
            {
                await _context.MainController
                    .Where(c => c.Id == controllerId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.PumpRelayStatus, pumpRelayStatus));
                return true; 
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool?> GetLightStatusAsync(int controllerId)
        {
            try
            {
                var lightStatus = await _context.MainController
                    .Where(c => c.Id == controllerId)
                    .Select(c => (bool?)c.LightStatus)  
                    .FirstOrDefaultAsync();

                return lightStatus;
            }
            catch(Exception ex)
            {
                throw new Exception("Não encontrado", ex);
            }
        }


        public async Task<bool?> GetPumpRelayStatusAsync(int controllerId)
        {
            try
            {
                var pumpRelayStatus = await _context.MainController
                    .Where(c => c.Id == controllerId)
                    .Select(c => (bool?)c.PumpRelayStatus)  
                    .FirstOrDefaultAsync();

                return pumpRelayStatus;
            }
            catch (Exception ex)
            {
                throw new Exception("Não encontrado", ex);
            }
        }

    }
}

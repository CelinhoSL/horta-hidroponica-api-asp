using Horta.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Horta_Api.Infrastructure.Repositories
{
    public class MainMcuConfigRepository : IMainMcuConfigRepository
    {

        private readonly ConnectionContext _context = new ConnectionContext();


        public async Task<MainController> CreateAsync(MainController controller)
        {
            _context.MainController.Add(controller);
            await _context.SaveChangesAsync();
            return controller;
        }

        public async Task<MainController> GetMcuByIp(string ipAdress)
        {
            return await _context.MainController.FirstOrDefaultAsync(m => m.IpAddress == ipAdress);
        }

        public async Task<MainController> DeleteMcuAsync(int controllerId)
        {
            var controller = await _context.MainController.FindAsync(controllerId);
            if (controller != null)
            {
                _context.MainController.Remove(controller);
                await _context.SaveChangesAsync();
            }
            return controller;
        }

        public async Task<int> GetMcuIdByUserId(int userId)
        {
            var mainController = await _context.MainController
                .FirstOrDefaultAsync(m => m.UserId == userId);

            return mainController.Id;
        }
    }
}

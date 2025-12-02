using Horta_Api.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Horta_Api.Infrastructure.Repositories
{
    public class CameraRepository : ICameraRepository
    {
        private readonly ConnectionContext _context = new ConnectionContext();

        public async Task<int> GetCameraIdAsync(int mainControllerId)
        {
            var camera = await _context.Set<Camera>()
                .FirstOrDefaultAsync(c => c.MainControllerId == mainControllerId);

            return camera?.Id ?? 0;
        }

        public async Task<Camera> CreateCameraAsync(int mainControllerId)
        {
            var camera = Camera.Create(mainControllerId);
            _context.Camera.Add(camera);
            await _context.SaveChangesAsync(); 
            return camera; 
        }

    }
}

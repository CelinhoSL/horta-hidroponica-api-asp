using Horta_Api.Domain.Model;
using Horta_Api.Infrastructure.Repositories;

namespace Horta_Api.Application.Services
{
    public class CameraService : ICameraService
    {
        private readonly ICameraRepository _cameraRepository;

        public CameraService(ICameraRepository cameraRepository)
        {
            _cameraRepository = cameraRepository;
        }

        public async Task<int> GetCameraIdAsync(int mainControllerId)
        {
            
            if (mainControllerId <= 0)
                throw new ArgumentException("Invalid UserId");

            return await _cameraRepository.GetCameraIdAsync(mainControllerId);
        }

        public async Task<Camera> CreateCameraAsync(int mainControllerId)
        {
            return await _cameraRepository.CreateCameraAsync(mainControllerId);
        }
    }
}

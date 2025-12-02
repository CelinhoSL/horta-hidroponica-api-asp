using Horta_Api.Domain.Model;

namespace Horta_Api.Application.Services
{
    public interface ICameraService
    {
        Task<int> GetCameraIdAsync(int mainControllerId);
        Task<Camera> CreateCameraAsync(int mainControllerId);
    }
}

using Horta_Api.Domain.Model;

namespace Horta_Api.Infrastructure.Repositories
{
    public interface ICameraRepository
    {
        Task<int> GetCameraIdAsync(int mainControllerId);
        Task<Camera> CreateCameraAsync(int mainControllerId);
        
    }
}

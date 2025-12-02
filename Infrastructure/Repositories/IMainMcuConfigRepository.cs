using Horta.Domain.Model;
using Horta_Api.Domain.DTOs;

namespace Horta_Api.Infrastructure.Repositories
{
    public interface IMainMcuConfigRepository
    {

        Task<MainController> CreateAsync(MainController controller);
        Task<int> GetMcuIdByUserId(int userId);
        Task<MainController> GetMcuByIp(string ipAdress);
        Task<MainController> DeleteMcuAsync(int controllerId);




    }
}

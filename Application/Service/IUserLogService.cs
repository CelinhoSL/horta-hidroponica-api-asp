using Horta.Domain.Model;

namespace Horta_Api.Application.Service
{
    public interface IUserLogService
    {
        public Task<UserLog> CreateAsync(UserLog userlog);
        public Task<UserLog> GetByIdAsync(int logId);
    }
}

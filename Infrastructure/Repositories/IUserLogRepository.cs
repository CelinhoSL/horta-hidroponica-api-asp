using Horta.Domain.Model;

namespace Horta_Api.Infrastructure.Repositories
{
    public interface IUserLogRepository
    {
        public Task<UserLog> CreateAsync(UserLog userlog);
        public Task<UserLog> GetByIdAsync(int logId);
    }
}

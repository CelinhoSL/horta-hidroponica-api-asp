using Horta.Domain.Model;
using Horta_Api.Infrastructure.Repositories;

namespace Horta_Api.Application.Service
{
    public class UserLogService : IUserLogService
    {
        private readonly IUserLogRepository _userLogRepository;

        public UserLogService(IUserLogRepository userLogRepository)
        {
            _userLogRepository = userLogRepository;
        }

        public async Task<UserLog> CreateAsync(UserLog userLog)
        {
            return await _userLogRepository.CreateAsync(userLog);
        }

        public async Task<UserLog> GetByIdAsync(int logId)
        {
            return await _userLogRepository.GetByIdAsync(logId);

        }
    }
}

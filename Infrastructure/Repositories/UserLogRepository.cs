using Horta.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Horta_Api.Infrastructure.Repositories
{
    public class UserLogRepository : IUserLogRepository
    {
        public readonly ConnectionContext _context = new ConnectionContext();

        public async Task<UserLog> CreateAsync(UserLog userlog)
        {
            _context.UserLog.Add(userlog);
            try
            {
                await _context.SaveChangesAsync();
                return userlog;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving user log", ex);
            }
        }

        public async Task<UserLog> GetByIdAsync(int logId)
        {
            try
            {
                return await _context.UserLog.FirstOrDefaultAsync(ul => ul.Id == logId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user log with ID {logId}", ex);
            }
        }
    }
}

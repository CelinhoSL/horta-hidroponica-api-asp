using Horta_Api.Application.Service;
using Horta.Domain.Model;
using Microsoft.EntityFrameworkCore;


namespace Horta_Api.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly ConnectionContext _context = new ConnectionContext();

        private readonly List<User> _users = [];

        public async Task<User> CreateAsync(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            return Task.FromResult(_users.Any(u => u.Email == email));
        }

        public async Task<User> GetEmailAsync(string email)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Email == email);

        }

        public async Task<User> GetByIdAsync(int userId)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserWithoutPasswordAsync(int userId, string newPassword)
        {
            var existingUser = await _context.User.FindAsync(userId);

            if (existingUser == null)
                throw new Exception("User not found"); 

            existingUser.Password = newPassword;
            existingUser.UpdatedAt = DateTime.UtcNow; 

            _context.User.Update(existingUser);
            await _context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<User> DeleteUserAsync(int userId)
        {
            var user = await _context.User.FindAsync(userId);
            if (user != null)
            {
                _context.User.Remove(user);
                await _context.SaveChangesAsync();
            }
            return user;
        }
    }
}

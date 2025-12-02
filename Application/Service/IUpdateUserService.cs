using Horta.Domain.Model;

namespace Horta_Api.Application.Service
{
    public interface IUpdateUserService
    {
        public Task <User> UpdateUserAsync(string currentPassword, User user, string currentHashPassword);
        public Task <User> UpdateUserWithoutPasswordAsync(int userId, string newPassoword);
    }
}

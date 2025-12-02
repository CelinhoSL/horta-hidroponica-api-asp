using Horta.Domain.Model;
using Horta_Api.Infrastructure.Repositories;
using Horta_Api.Infrastructure.Security;

namespace Horta_Api.Application.Service
{
    public class UpdateUserService : IUpdateUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UpdateUserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }


        // the update can only be done once the user indetified themself with the password

        public async Task<User> UpdateUserAsync(string currentPassword, User user,string currentHashPassword)
        {
            if (!_passwordHasher.VerifyPassword(currentPassword, currentHashPassword))
            {
                throw new UnauthorizedAccessException("Credenciais inválidas");
            }
            await _userRepository.UpdateAsync(user);
            return user;
        }

        public async Task<User> UpdateUserWithoutPasswordAsync(int userId, string newPassoword)
        {
            return await _userRepository.UpdateUserWithoutPasswordAsync(userId, newPassoword);
            
        }
    }

}


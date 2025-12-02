using Horta.Domain.Model;
using Horta_Api.Domain.DTOs;

namespace Horta_Api.Application.Service
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(CreateUserDto createUserDto);
        Task<bool> ExistsEmail(string email);
        Task<UserLoginResponseDto> LoginAsync(UserLoginDto loginDto);
        Task<User> GetUserByIdAsync(int userId);
        Task<User> UpdateAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User> DeleteUserAsync(int userId);

    }
}
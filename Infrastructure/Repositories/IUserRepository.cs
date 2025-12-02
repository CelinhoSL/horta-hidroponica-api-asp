// Application/Services/UserService.cs

using Horta.Domain.Model;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User> GetEmailAsync(string email);
    Task<User> GetByIdAsync(int userId);
    Task<User> UpdateAsync(User user);
    Task<User> GetByEmailAsync(string email);
    Task<User> UpdateUserWithoutPasswordAsync(int userId, string newPassoword);
    Task<User> DeleteUserAsync(int userId);
}
using Horta.Domain.Model;
using Horta_Api.Application.Service;
using Horta_Api.Domain.DTOs;
using Horta_Api.Infrastructure.Security;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    
    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> CreateUserAsync(CreateUserDto createUserDto)
    {
        
        createUserDto.Validate();

        
        if (await _userRepository.ExistsByEmailAsync(createUserDto.Email))
            throw new InvalidOperationException("Email já está em uso");

        
        var user = new User()
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            IpAddress = createUserDto.IpAddress,
            UserAgent = createUserDto.UserAgent,
            Password = _passwordHasher.HashPassword(createUserDto.Password),
            CreatedAt = DateTime.UtcNow
        };

        
        var createdUser = await _userRepository.CreateAsync(user);

        
        return createdUser;


    }

    public async Task<UserLoginResponseDto> LoginAsync(UserLoginDto loginDto)
    {
        var user = await _userRepository.GetEmailAsync(loginDto.Email);
        
        if (user == null)
            throw new InvalidOperationException("Usuário não encontrado");


        if (!_passwordHasher.VerifyPassword(loginDto.Password, user.Password))
            throw new UnauthorizedAccessException("Credenciais inválidas");

        var token = TokenService.GenerateToken(user);

        return new UserLoginResponseDto
        {
            UserId = user.UserId,
            Username = user.Username
        };
    }

    public async Task<bool> ExistsEmail(string email)
    {
        var user = await _userRepository.GetEmailAsync(email);
        if (user != null)
        {
            return false; // Email já existe
        }
        else
        {
            return true; // Email não existe
        }
    }
         public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId) 
                ?? throw new KeyNotFoundException("Usuário não encontrado");
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User> UpdateAsync(User user)
    {
        return await _userRepository.UpdateAsync(user);
    }

        public async Task<User> DeleteUserAsync(int userId)
        {
            
            return await _userRepository.DeleteUserAsync(userId);
    }

}

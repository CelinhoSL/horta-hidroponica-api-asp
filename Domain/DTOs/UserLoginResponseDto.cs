namespace Horta_Api.Domain.DTOs
{
    public class UserLoginResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
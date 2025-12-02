using Horta_Api.Aplication.Service.Validators;
using Horta_Api.Application.Service.Validators;
using System.Text.Json.Serialization;

namespace Horta_Api.Domain.DTOs
{
    public class CreateUserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public string? IpAddress { get; set; }
        [JsonIgnore]
        public string? UserAgent { get; set; }
        public void Validate()
        {
            UsernameValidator.Validate(Username);
            PasswordValidator.Validate(Password);
            EmailValidator.Validate(Email);
            

        }
    }

}

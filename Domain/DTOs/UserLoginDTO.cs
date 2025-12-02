using Horta_Api.Aplication.Service.Validators;
using Horta_Api.Application.Service.Validators;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Horta_Api.Domain.DTOs
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [JsonIgnore]
        public string? IpAddress { get; set; }
        [JsonIgnore]
        public string? UserAgent { get; set; }

        public void Validate()
        {
            PasswordValidator.Validate(Password);
            EmailValidator.Validate(Email);
        }
    }

}
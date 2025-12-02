using Horta_Api.Aplication.Service.Validators;

namespace Horta_Api.Domain.DTOs
{
    public class UpdateUserDTO
    {
        public string CurrentPassword {  get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }

        public void Validate()
        {
            

            if (!string.IsNullOrWhiteSpace(Username))
            {
                UsernameValidator.Validate(Username);
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                PasswordValidator.Validate(Password);
            }

            if (!string.IsNullOrWhiteSpace(Email))
            {
                EmailValidator.Validate(Email);
            }
        }
    }
}

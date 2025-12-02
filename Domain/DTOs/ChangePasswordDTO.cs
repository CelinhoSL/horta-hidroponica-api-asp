using Horta_Api.Aplication.Service.Validators;

namespace Horta_Api.Domain.DTOs
{
    public class ChangePasswordDTO
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }

        public void Validate()
        {
            EmailValidator.Validate(Email);
            PasswordValidator.Validate(NewPassword);
        }
    }
}

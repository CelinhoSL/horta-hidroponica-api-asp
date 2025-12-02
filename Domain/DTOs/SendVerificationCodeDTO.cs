using Horta_Api.Aplication.Service.Validators;

namespace Horta_Api.Domain.DTOs
{
    public class SendVerificationCodeDTO
    {
        public string Email { get; set; }

        public void Validate()
        {
            EmailValidator.Validate(Email);
        }

    }
}

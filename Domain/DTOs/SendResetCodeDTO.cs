using Horta_Api.Aplication.Service.Validators;

namespace Horta_Api.Domain.DTOs
{
    public class SendResetCodeDTO
    {
    public string Email { get; set; }

        public void Validate()
        {

            EmailValidator.Validate(Email);
            
        }
    }
}

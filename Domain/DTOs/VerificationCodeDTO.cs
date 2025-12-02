using Horta_Api.Application.Service.Validators;

namespace Horta_Api.Domain.DTOs
{
    public class VerificationCodeDTO
    {
        public int Code { get; set; }

        public void Validate()
        {
            VerificationCodeFormat.Validate(Code);
        }
    }
}

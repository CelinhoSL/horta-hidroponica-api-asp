namespace Horta_Api.Application.Service.Validators
{
    public class VerificationCodeFormat
    {
        public static void Validate(int verificationCode)
        {
            if (verificationCode < 100000 || verificationCode > 999999)
            {
                throw new ArgumentException("O código de verificação deve ser um número de 6 dígitos.");
            }
        }
    }
}

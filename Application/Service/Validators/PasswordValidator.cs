using System;
using System.Linq;

namespace Horta_Api.Aplication.Service.Validators
{
    public class PasswordValidator
    {
        public static void Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("A senha não pode estar vazia.");

            if (!IsValidLength(password))
                throw new Exception("A senha deve ter no mínimo 8 caracteres.");

            if (!HasSpecialChar(password))
                throw new Exception("A senha deve conter pelo menos um caractere especial.");

            if (!HasNumericDigit(password))
                throw new Exception("A senha deve conter pelo menos um número.");
        }

        public static bool IsValidLength(string password)
        {
            return password.Length >= 8;
        }

        public static bool HasSpecialChar(string password)
        {
            return password.Any(ch => !char.IsLetterOrDigit(ch));
        }

        public static bool HasNumericDigit(string password)
        {
            return password.Any(char.IsDigit);
        }
    }
}

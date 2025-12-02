using System;
using System.Linq;

namespace Horta_Api.Aplication.Service.Validators
{
    public class UsernameValidator
    {
        public static void Validate(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new Exception("O nome de usuário não pode estar vazio.");

            if (!IsValidLength(username))
                throw new Exception("O nome de usuário deve ter no mínimo 3 caracteres.");

            if (!IsValidChars(username))
                throw new Exception("O nome de usuário deve conter apenas letras e números.");
        }

        private static bool IsValidLength(string username)
        {
            return username.Length >= 3;
        }

        private static bool IsValidChars(string username)
        {
            return username.All(char.IsLetterOrDigit); 

        }
    }
}

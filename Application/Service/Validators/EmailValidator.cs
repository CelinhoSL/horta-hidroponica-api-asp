namespace Horta_Api.Aplication.Service.Validators
{
    public class EmailValidator
    {
        public static void Validate(string email)
        {
            if (!IsValidEmailFormat(email))
                throw new Exception("Formato de e-mail inválido.");

            if (!IsValidDomain(email))
                throw new Exception("Domínio de e-mail não permitido.");
        }

        private static bool IsValidEmailFormat(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
               
                return false;
            }
        }

        private static bool IsValidDomain(string email)
        {
            var allowedDomains = new List<string> { "gmail.com", "outlook.com", "hotmail.com" };
            var domain = email.Split('@').LastOrDefault();
            return domain != null && allowedDomains.Contains(domain);
        }
    }
}

namespace Horta_Api.Aplication.Service
{
    public class GenerateUserCode
    {
        public int Code { get; private set; }
        public string Email { get; private set; }

        public GenerateUserCode(string email)
        {
            Email = email;
            Code = GenerateCode();
        }

        private int GenerateCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }

        
        public (int, string) GetCodeAndEmail()
        {
            return (Code, Email);
        }
    }
}

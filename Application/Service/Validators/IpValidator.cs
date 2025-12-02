using System.Net;
using System.Net.Sockets;

namespace Horta_Api.Application.Service.Validators
{
    public static class IpValidator
    {
        public static void Validate(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new ArgumentException("O endereço IP não pode estar vazio ou nulo.");

            if (!IPAddress.TryParse(ip, out var address))
                throw new FormatException("Formato de endereço IP inválido.");

            // (Opcional) Se quiser restringir só a IPv4:
            if (address.AddressFamily != AddressFamily.InterNetwork)
                throw new FormatException("Apenas endereços IPv4 são permitidos.");
        }
    }
}

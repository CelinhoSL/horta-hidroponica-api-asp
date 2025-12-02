using Horta_Api.Application.Service.Validators;
using System.Text.Json.Serialization;

namespace Horta_Api.Domain.DTOs
{
    public class InitializeMainControllerDTO
    {
        [JsonIgnore]
        public int UserId { get; set; }
        public string ipAddress { get; set; }
        public string DeviceSecretKey { get; set; }


        public void Validate()
        {
            //IpValidator.Validate(ipAddress);
        }


    }
}

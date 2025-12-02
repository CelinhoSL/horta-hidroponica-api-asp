namespace Horta_Api.Domain.DTOs
{
    public class McuLoginDTO
    {
        public string IpAddress { get; set; }
        public string DeviceSecretKey { get; set; }

        public void Validate()
        {

        }
    }
}

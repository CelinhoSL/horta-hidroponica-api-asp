using System.Net.WebSockets;

namespace Horta_Api.Application.Interfaces
{
    public interface IGardenGetSensorValue
    {
        Task HandleAsync(WebSocket socket, int controllerId);
        Task BroadcastAsync(int controllerId, object data);
    }
}

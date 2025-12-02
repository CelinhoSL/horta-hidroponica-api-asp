using System.Net.WebSockets;

namespace Horta_Api.Application.Interfaces
{
    public interface ISensorUpdater
    {
        Task HandleAsync(WebSocket socket, int controllerId);
    }
}

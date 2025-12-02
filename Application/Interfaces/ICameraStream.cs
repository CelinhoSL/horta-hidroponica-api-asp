using System.Net.WebSockets;

namespace Horta_Api.Application.Interfaces
{
    public interface ICameraStream
    {
        Task HandleConnectionAsync(WebSocket socket, int userId, int camId, string clientType);
        Task BroadcastFrameAsync(int camId, byte[] frameData);
        int GetViewerCount(int camId);
        bool HasActiveSender(int camId);
    }
}

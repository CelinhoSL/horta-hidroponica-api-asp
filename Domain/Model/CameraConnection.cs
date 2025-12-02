using System.Net.WebSockets;

namespace Horta_Api.Domain.Model
{
    public class CameraConnection
    {
        public string ConnectionId { get; set; } = Guid.NewGuid().ToString();
        public WebSocket Socket { get; set; } = null!;
        public int UserId { get; set; }
        public int CamId { get; set; }
        public string ClientType { get; set; } = string.Empty; // "sender" ou "viewer"
        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastFrameAt { get; set; } = DateTime.UtcNow;
        public long FramesReceived { get; set; } = 0;
        public long BytesReceived { get; set; } = 0;

        public bool IsSender => ClientType == "sender";
        public bool IsViewer => ClientType == "viewer";
        public bool IsActive => Socket?.State == WebSocketState.Open;
    }
}

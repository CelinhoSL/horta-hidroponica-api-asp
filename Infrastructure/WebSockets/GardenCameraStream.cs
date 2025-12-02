using Horta_Api.Application.Interfaces;
using Horta_Api.Domain.Model;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Horta_Api.Infrastructure.WebSockets
{
    public class GardenCameraStream : ICameraStream
    {
        private readonly ConcurrentDictionary<int, ConcurrentBag<CameraConnection>> _connections =
                new ConcurrentDictionary<int, ConcurrentBag<CameraConnection>>();


        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public GardenCameraStream(IServiceProvider serviceProvider, ILogger<GardenCameraStream> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task HandleConnectionAsync(WebSocket socket, int userId, int camId, string clientType)
        {
            if (clientType != "sender" && clientType != "viewer")
            {
                await CloseSocketAsync(socket, WebSocketCloseStatus.PolicyViolation, "Tipo inválido");
                return;
            }

            var connection = new CameraConnection
            {
                Socket = socket,
                UserId = userId,
                CamId = camId,
                ClientType = clientType
            };

            var connections = _connections.GetOrAdd(camId, _ => new ConcurrentBag<CameraConnection>());
            connections.Add(connection);

            _logger.LogInformation("WebSocket connection: UserId={UserId}, CamId={CamId}, Type={Type}",
            userId, camId, clientType);

            try
            {
                if (connection.IsSender)
                {
                    await HandleSenderAsync(connection);
                }
                else
                {
                    await HandleViewerAsync(connection);
                }
            }
            catch (WebSocketException wsEx)
            {
                _logger.LogWarning(wsEx, "WebSocket disconnected: CamId={CamId}", camId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebSocket error: CamId={CamId}", camId);
            }
            finally
            {
                await RemoveConnectionAsync(connection);
            }
        }

        private async Task HandleSenderAsync(CameraConnection sender)
        {
            var buffer = new byte[1024 * 256]; 

            while (sender.Socket.State == WebSocketState.Open)
            {
                var result = await sender.Socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    sender.FramesReceived++;
                    sender.BytesReceived += result.Count;
                    sender.LastFrameAt = DateTime.UtcNow;

                    var frameData = new byte[result.Count];
                    Array.Copy(buffer, frameData, result.Count);

                    await BroadcastFrameAsync(sender.CamId, frameData);
                }
            }

            await CloseSocketAsync(sender.Socket, WebSocketCloseStatus.NormalClosure, "Sender desconectado");
        }

        private async Task HandleViewerAsync(CameraConnection viewer)
        {
            // Enviar mensagem de boas-vindas
            var welcomeMessage = System.Text.Encoding.UTF8.GetBytes(
                $"{{\"type\":\"connected\",\"camId\":{viewer.CamId},\"message\":\"Conectado com sucesso\"}}");

            if (viewer.Socket.State == WebSocketState.Open)
            {
                await viewer.Socket.SendAsync(
                    new ArraySegment<byte>(welcomeMessage),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }

            var buffer = new byte[1024];
            try
            {
                while (viewer.Socket.State == WebSocketState.Open)
                {
                    var result = await viewer.Socket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                    }
                }
            }
            catch (WebSocketException)
            {
            }

            await CloseSocketAsync(viewer.Socket, WebSocketCloseStatus.NormalClosure, "Viewer desconectado");
        }

        public async Task BroadcastFrameAsync(int camId, byte[] frameData)
        {
            if (!_connections.TryGetValue(camId, out var connections))
            {
                return;
            }

            var viewers = connections
                .Where(c => c.IsViewer && c.IsActive)
                .ToList();

            if (!viewers.Any())
            {
                return;
            }

            var sendTasks = viewers.Select(async viewer =>
            {
                try
                {
                    if (viewer.Socket.State == WebSocketState.Open)
                    {
                        await viewer.Socket.SendAsync(
                            new ArraySegment<byte>(frameData),
                            WebSocketMessageType.Binary,
                            true,
                            CancellationToken.None);

                        viewer.FramesReceived++;
                    }
                }
                catch (Exception)
                {
                }
            });

            await Task.WhenAll(sendTasks);
        }

        public int GetViewerCount(int camId)
        {
            if (!_connections.TryGetValue(camId, out var connections))
                return 0;

            return connections.Count(c => c.IsViewer && c.IsActive);
        }

        public bool HasActiveSender(int camId)
        {
            if (!_connections.TryGetValue(camId, out var connections))
                return false;

            return connections.Any(c => c.IsSender && c.IsActive);
        }

        private async Task RemoveConnectionAsync(CameraConnection connection)
        {
            if (_connections.TryGetValue(connection.CamId, out var connections))
            {
                var remainingConnections = connections.Where(c => c.ConnectionId != connection.ConnectionId).ToList();

                if (remainingConnections.Any())
                {
                    _connections[connection.CamId] = new ConcurrentBag<CameraConnection>(remainingConnections);
                }
                else
                {
                    _connections.TryRemove(connection.CamId, out _);
                }
            }

            await CloseSocketAsync(connection.Socket, WebSocketCloseStatus.NormalClosure, "Conexão encerrada");
        }

        private async Task CloseSocketAsync(WebSocket socket, WebSocketCloseStatus status, string description)
        {
            if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
            {
                try
                {
                    await socket.CloseAsync(status, description, CancellationToken.None);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
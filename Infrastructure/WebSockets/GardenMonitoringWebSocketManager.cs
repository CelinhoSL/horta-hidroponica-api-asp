using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;
using Horta_Api.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Horta_Api.Infrastructure.WebSockets
{
    public class GardenMonitoringWebSocketManager : IGardenMonitoringAndControllingWebSocketManager
    {
        private readonly ConcurrentDictionary<int, ConcurrentBag<WebSocket>> _connections =
            new ConcurrentDictionary<int, ConcurrentBag<WebSocket>>();
        private readonly IServiceProvider _serviceProvider;
        private readonly int _updateIntervalMs = 1000; 

        public GardenMonitoringWebSocketManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task HandleAsync(WebSocket socket, int controllerId)
        {
            var sockets = _connections.GetOrAdd(controllerId, _ => new ConcurrentBag<WebSocket>());
            sockets.Add(socket);

            var cts = new CancellationTokenSource();

            var updateTask = Task.Run(async () =>
            {
                while (socket.State == WebSocketState.Open && !cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var mainMcuService = scope.ServiceProvider.GetRequiredService<IMainMcuService>();

                        var lightStatus = await mainMcuService.GetLightStatusAsync(controllerId);
                        var pumpStatus = await mainMcuService.GetPumpRelayStatusAsync(controllerId);

                        var data = new
                        {
                            lightStatus,
                            pumpRelayStatus = pumpStatus,
                            timestamp = DateTime.UtcNow
                        };

                        var message = JsonSerializer.Serialize(data);
                        var bytes = Encoding.UTF8.GetBytes(message);

                        if (socket.State == WebSocketState.Open)
                        {
                            await socket.SendAsync(
                                new ArraySegment<byte>(bytes),
                                WebSocketMessageType.Text,
                                true,
                                cts.Token
                            );
                        }

                        await Task.Delay(_updateIntervalMs, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao enviar atualização: {ex.Message}");
                        break;
                    }
                }
            }, cts.Token);

            var buffer = new byte[1024 * 4];
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        CancellationToken.None
                    );

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        cts.Cancel();
                        await socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "",
                            CancellationToken.None
                        );
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    }
                }
            }
            finally
            {
                cts.Cancel();
                await updateTask;

                if (_connections.TryGetValue(controllerId, out var connectedSockets))
                {
                    var updatedSockets = new ConcurrentBag<WebSocket>(
                        connectedSockets.Where(s => s != socket)
                    );
                    _connections.TryUpdate(controllerId, updatedSockets, connectedSockets);
                }
            }
        }

        public async Task BroadcastAsync(int controllerId, object data)
        {
            if (!_connections.TryGetValue(controllerId, out var sockets))
                return;

            var message = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(message);

            var socketsList = sockets.ToList();
            var tasks = new List<Task>();

            foreach (var socket in socketsList)
            {
                if (socket.State == WebSocketState.Open)
                {
                    tasks.Add(socket.SendAsync(
                        new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    ));
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}
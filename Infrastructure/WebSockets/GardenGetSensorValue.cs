using Horta_Api.Application.Interfaces;
using Horta_Api.Application.Services;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Horta_Api.Infrastructure.WebSockets
{
    public class GardenGetSensorValue : IGardenGetSensorValue
    {
        private readonly ConcurrentDictionary<int, ConcurrentBag<WebSocket>> _connections =
            new ConcurrentDictionary<int, ConcurrentBag<WebSocket>>();

        private readonly IServiceProvider _serviceProvider;
        public GardenGetSensorValue(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task HandleAsync(WebSocket socket, int controllerId)
        {
            var sockets = _connections.GetOrAdd(controllerId, _ => new ConcurrentBag<WebSocket>());
            sockets.Add(socket);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var lightSensorService = scope.ServiceProvider.GetRequiredService<ILightSensorService>();
                var waterLevelSensorService = scope.ServiceProvider.GetRequiredService<IWaterLevelSensorService>();
                var temperatureSensorService = scope.ServiceProvider.GetRequiredService<ITemperatureSensorService>();

                while (socket.State == WebSocketState.Open)
                {
                    var lightSensorId = await lightSensorService.GetLightSensorIdByMCUIdAsync(controllerId);
                    var lightSensorValue = await lightSensorService.GetLightValueAsync(lightSensorId);

                    var temperatureSensorId = await temperatureSensorService.GetTemperatureSensorIdByMCUIdAsync(controllerId);
                    var temperatureSensorValue = await temperatureSensorService.GetTemperatureValueAsync(temperatureSensorId);

                    var waterSensorId = await waterLevelSensorService.GetWaterLevelSensorByMCUIdAsync(controllerId);
                    var waterSensorValue = await waterLevelSensorService.GetWaterLevelValueAsync(waterSensorId);

                    var data = new
                    {
                        lightSensorValue,
                        waterSensorValue,
                        temperatureSensorValue,
                    };

                    var message = JsonSerializer.Serialize(data);
                    var bytes = Encoding.UTF8.GetBytes(message);

                    await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);

                    await Task.Delay(2000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WebSocket Error] {ex.Message}");
                if (socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Erro interno", CancellationToken.None);
                }
            }
        }


        public async Task BroadcastAsync(int controllerId, object data)
        {
            if (!_connections.TryGetValue(controllerId, out var sockets)) return;

            var message = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(message);

            foreach (var socket in sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}

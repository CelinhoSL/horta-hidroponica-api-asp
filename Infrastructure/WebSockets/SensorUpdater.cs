using Horta_Api.Application.Interfaces;
using Horta_Api.Application.Services;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Horta_Api.Infrastructure.WebSockets
{
    public class SensorUpdater : ISensorUpdater
    {
        private readonly IServiceProvider _serviceProvider;

        public SensorUpdater(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task HandleAsync(WebSocket socket, int controllerId)
        {
            using var scope = _serviceProvider.CreateScope();
            var lightService = scope.ServiceProvider.GetRequiredService<ILightSensorService>();
            var waterService = scope.ServiceProvider.GetRequiredService<IWaterLevelSensorService>();
            var temperatureService = scope.ServiceProvider.GetRequiredService<ITemperatureSensorService>();

            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var doc = JsonDocument.Parse(json);

                    var type = doc.RootElement.GetProperty("type").GetString();
                    var value = doc.RootElement.GetProperty("value").GetSingle();

                    switch (type.ToLower())
                    {
                        case "light":
                            await lightService.UpdateLightValueAsync(controllerId, value);
                            break;

                        case "water_level":
                            await waterService.UpdateWaterLevelValueAsync(controllerId, value);
                            break;

                        case "temperature":
                            await temperatureService.UpdateTemperatureValueAsync(controllerId, value);
                            break;
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                }
            }
        }
    }
}

using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;

namespace AutoPi.TelemetryApi;

public class TelemetryProcessor(
    ChannelReader<TelemetryPayload> reader,
    IHubContext<TelemetryHub> hubContext,
    ILogger<TelemetryProcessor> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var payload in reader.ReadAllAsync(stoppingToken))
        {
            logger.LogInformation(
                "[{Time}] {Device} -> RPM: {Rpm} | Vit: {Speed} km/h | Throttle: {Throttle}% | Coolant: {Coolant}°C | Load: {Load}%",
                payload.Timestamp,
                payload.DeviceId,
                payload.Metrics.EngineRpm?.ToString() ?? "N/A",
                payload.Metrics.VehicleSpeed?.ToString() ?? "N/A",
                payload.Metrics.ThrottlePosition?.ToString() ?? "N/A",
                payload.Metrics.CoolantTemperature?.ToString() ?? "N/A",
                payload.Metrics.EngineLoad?.ToString() ?? "N/A"
            );

            await hubContext.Clients.All.SendAsync("ReceiveTelemetry", payload, stoppingToken);
        }
    }
}

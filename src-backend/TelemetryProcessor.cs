using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;

namespace AutoPi.TelemetryApi;

public class TelemetryProcessor(
    ChannelReader<TelemetryPayload> reader,
    IHubContext<TelemetryHub> hubContext,
    DbChannelWriter dbWriter,
    ILogger<TelemetryProcessor> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("TelemetryProcessor démarré.");

        try
        {
            await foreach (var payload in reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    logger.LogInformation(
                        "[{Time}] {Device} -> RPM: {Rpm} | Vit: {Speed} km/h | Throttle: {Throttle}% | Coolant: {Coolant}°C | Load: {Load}%",
                        payload.Timestamp,
                        payload.DeviceId,
                        payload.Metrics.EngineRpm?.ToString()          ?? "N/A",
                        payload.Metrics.VehicleSpeed?.ToString()        ?? "N/A",
                        payload.Metrics.ThrottlePosition?.ToString()    ?? "N/A",
                        payload.Metrics.CoolantTemperature?.ToString()  ?? "N/A",
                        payload.Metrics.EngineLoad?.ToString()          ?? "N/A"
                    );

                    // Relais temps-réel vers le dashboard Blazor
                    await hubContext.Clients.All.SendAsync("ReceiveTelemetry", payload, stoppingToken);

                    // Fanout vers DatabaseProcessor
                    dbWriter.TryWrite(payload);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        "Erreur lors du traitement du payload (device: {Device}, timestamp: {Time}).",
                        payload.DeviceId, payload.Timestamp);
                }
            }

            logger.LogInformation("TelemetryProcessor arrêté proprement (channel fermé).");
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("TelemetryProcessor arrêté proprement (annulation demandée).");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Erreur fatale dans TelemetryProcessor. Le BackgroundService s'arrête.");
            throw;
        }
    }
}
 
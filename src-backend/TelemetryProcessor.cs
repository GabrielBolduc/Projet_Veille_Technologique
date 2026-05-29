using System.Threading.Channels;

namespace AutoPi.TelemetryApi;

public class TelemetryProcessor(ChannelReader<TelemetryPayload> reader, ILogger<TelemetryProcessor> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var payload in reader.ReadAllAsync(stoppingToken))
        {
            logger.LogInformation(
                "Payload reçu — device={DeviceId} ts={Timestamp} rpm={Rpm} speed={Speed}",
                payload.DeviceId,
                payload.Timestamp,
                payload.Metrics.EngineRpm,
                payload.Metrics.VehicleSpeed);
        }
    }
}

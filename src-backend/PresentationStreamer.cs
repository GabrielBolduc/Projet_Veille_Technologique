using System.Text.Json;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;

namespace AutoPi.TelemetryApi;

// service de streaming pour la présentation : lit les donne d'une session en base et les diffuse en boucle vers le dashboard Blazor via le channel
public class PresentationStreamer(
    IDbContextFactory<TelemetryDbContext> dbFactory,
    ChannelWriter<TelemetryPayload> writer,
    ILogger<PresentationStreamer> logger)
    : ITelemetryStreamer
{
    private CancellationTokenSource? _cts;

    public bool IsStreaming => _cts is { IsCancellationRequested: false };

    // démarre le streaming d'une session spécifique ou de toutes les sessions si sessionId est null.
    public Task StartAsync(int? sessionId = null)
    {
        if (IsStreaming) return Task.CompletedTask;

        _cts = new CancellationTokenSource();
        _ = Task.Run(() => RunAsync(sessionId, _cts.Token));
        logger.LogInformation("[Présentation] Streaming démarré (session={Id}).",
            sessionId?.ToString() ?? "toutes");
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _cts?.Cancel();
        _cts = null;
        logger.LogInformation("[Présentation] Streaming arrêté.");
        return Task.CompletedTask;
    }

    // boucle de lecture du channel
    private async Task RunAsync(int? sessionId, CancellationToken ct)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync(ct);

            var query = db.Records.AsQueryable();
            if (sessionId.HasValue)
                query = query.Where(r => r.TripSessionId == sessionId.Value);

            var records = await query
                .OrderBy(r => r.ReceivedAt)
                .Select(r => new {
                    r.DeviceId, r.EngineRpm, r.VehicleSpeed,
                    r.ThrottlePosition, r.EngineLoad, r.CoolantTemperature,
                    r.DtcPresent, r.DtcCodesJson
                })
                .ToListAsync(ct);

            if (records.Count == 0)
            {
                logger.LogWarning("[Présentation] Aucun enregistrement trouvé pour la session={Id}.",
                    sessionId?.ToString() ?? "toutes");
                return;
            }

            logger.LogInformation("[Présentation] {Count} enregistrements chargés, boucle infinie à 10 Hz.",
                records.Count);

            while (!ct.IsCancellationRequested)
            {
                foreach (var r in records)
                {
                    if (ct.IsCancellationRequested) break;

                    var payload = new TelemetryPayload(
                        r.DeviceId,
                        DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        new TelemetryMetrics(r.EngineRpm, r.VehicleSpeed, r.ThrottlePosition,
                                             r.EngineLoad, r.CoolantTemperature),
                        new TelemetryDiagnostics(r.DtcPresent,
                            JsonSerializer.Deserialize<List<string>>(r.DtcCodesJson) ?? [])
                    );

                    await writer.WriteAsync(payload, ct);
                    await Task.Delay(100, ct);
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Présentation] Erreur inattendue dans le flux.");
        }
    }
}

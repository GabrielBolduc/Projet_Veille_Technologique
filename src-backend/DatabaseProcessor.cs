namespace AutoPi.TelemetryApi;

public class DatabaseProcessor(
    DbChannelReader dbReader,
    IServiceProvider services,
    ILogger<DatabaseProcessor> logger)
    : BackgroundService
{
    private int?  _currentSessionId;
    private readonly List<TelemetryRecord> _batch = new();
    private DateTime _lastSave = DateTime.UtcNow;

    private const int BatchSize = 30;
    private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(3);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[DB] DatabaseProcessor démarré.");
        try
        {
            await foreach (var payload in dbReader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await HandleAsync(payload, stoppingToken);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[DB] Erreur sur le payload {Device}.", payload.DeviceId);
                }
            }
            logger.LogInformation("[DB] DatabaseProcessor arrêté proprement.");
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("[DB] DatabaseProcessor arrêté (annulation).");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "[DB] Erreur fatale dans DatabaseProcessor.");
            throw;
        }
        finally
        {
            if (_batch.Count > 0)
                await FlushAsync(CancellationToken.None);
        }
    }

    private async Task HandleAsync(TelemetryPayload p, CancellationToken ct)
    {
        var rpm = p.Metrics.EngineRpm ?? 0;

        // Ouverture de session
        if (_currentSessionId is null && rpm > 500)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
            var session = new TripSession { DeviceId = p.DeviceId, StartTime = DateTime.UtcNow };
            db.Sessions.Add(session);
            await db.SaveChangesAsync(ct);
            _currentSessionId = session.Id;
            logger.LogInformation("[DB] Session {Id} démarrée ({Device}).", session.Id, p.DeviceId);
        }

        // Accumulation dans le batch
        if (_currentSessionId is not null)
            _batch.Add(TelemetryRecord.FromPayload(p, _currentSessionId.Value));

        // Flush conditionnel (30 enregistrements OU 3 secondes)
        if (_batch.Count > 0 &&
            (_batch.Count >= BatchSize || DateTime.UtcNow - _lastSave >= FlushInterval))
        {
            await FlushAsync(ct);
        }

        // Fermeture de session (RPM retombe à 0)
        if (_currentSessionId is not null && rpm == 0)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
            var session = await db.Sessions.FindAsync([_currentSessionId.Value], ct);
            if (session is not null)
            {
                session.EndTime = DateTime.UtcNow;
                await db.SaveChangesAsync(ct);
            }
            logger.LogInformation("[DB] Session {Id} clôturée.", _currentSessionId);
            _currentSessionId = null;
        }
    }

    private async Task FlushAsync(CancellationToken ct)
    {
        try
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
            db.Records.AddRange(_batch);
            await db.SaveChangesAsync(ct);
            logger.LogInformation("[DB] {Count} enregistrement(s) persisté(s).", _batch.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[DB] Erreur lors du flush ({Count} enregistrements).", _batch.Count);
        }
        finally
        {
            _batch.Clear();
            _lastSave = DateTime.UtcNow;
        }
    }
}

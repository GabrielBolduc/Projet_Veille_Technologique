using Microsoft.EntityFrameworkCore;

namespace AutoPi.TelemetryApi;

// service de fond qui lit les payloads du channel, les envoie vers le dashboard Blazor avec SignalR, et les envoi vers la base de données
public class DatabaseProcessor(
    DbChannelReader dbReader,
    IServiceProvider services,
    ILogger<DatabaseProcessor> logger)
    : BackgroundService
{
    // état interne du suivi de session
    private volatile bool _isTrackingActive = false; 
    private int?  _currentSessionId;
    private string _pendingTitle = "";
    private readonly List<TelemetryRecord> _batch = new();
    private DateTime _lastSave = DateTime.UtcNow;

    private const int BatchSize = 30;
    private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(3);

    public bool IsTrackingActive => _isTrackingActive;

    // Démarre une nouvelle session de conduite ou reprend une session existante non supprimer.
    public async Task StartTrackingAsync(string title)
    {
        if (_isTrackingActive) return;

        _pendingTitle = title;

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();

        var existing = await db.Sessions.FirstOrDefaultAsync(s => s.EndTime == null);
        if (existing is not null)
        {
            _currentSessionId = existing.Id;
            logger.LogInformation("[DB] Reprise du trajet {Id} ({Device}).", existing.Id, existing.DeviceId);
        }
        else
        {
            _currentSessionId = null; // sera créée au premier payload
        }

        _isTrackingActive = true;
        logger.LogInformation("[DB] Suivi démarré : {Title}.", title);
    }

    // Termine la session en cours flush les donnee
    public async Task CloseCurrentSessionAsync()
    {
        if (!_isTrackingActive) return;
        _isTrackingActive = false;

        if (_batch.Count > 0)
            await FlushAsync(CancellationToken.None);

        if (_currentSessionId is not null)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
            var session = await db.Sessions.FindAsync([_currentSessionId.Value]);
            if (session is not null)
            {
                session.EndTime = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
            logger.LogInformation("[DB] Session {Id} archivée.", _currentSessionId);
            _currentSessionId = null;
        }
    }

    // Reprend une session archivée : efface EndTime et réactive le suivi.
    public async Task ResumeSessionAsync(int id)
    {
        if (_isTrackingActive) return;

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
        var session = await db.Sessions.FindAsync([id]);
        if (session is null) return;

        session.EndTime = null;
        await db.SaveChangesAsync();

        _currentSessionId = id;
        _pendingTitle     = session.Title;
        _isTrackingActive = true;
        logger.LogInformation("[DB] Session {Id} reprise : {Title}.", id, session.Title);
    }

    // Supprime un trajet et tous ses enregistrements (cascade).
    public async Task DeleteSessionAsync(int id)
    {
        if (_currentSessionId == id)
            await CloseCurrentSessionAsync();

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
        var session = await db.Sessions.FindAsync([id]);
        if (session is not null)
        {
            db.Sessions.Remove(session);
            await db.SaveChangesAsync();
            logger.LogInformation("[DB] Session {Id} supprimée (cascade).", id);
        }
    }

    // boucle de lecture du channel
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
    // traite un payload : création de session si besoin, ajout au batch, flush si seuil atteint
    private async Task HandleAsync(TelemetryPayload p, CancellationToken ct)
    {
        if (!_isTrackingActive) return;

        // Création de session au premier payload si pas de reprise existante
        if (_currentSessionId is null)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
            // Le titre choisi par l'utilisateur a la priorité ; sinon on tente la résolution par VIN.
            string finalTitle = string.IsNullOrWhiteSpace(_pendingTitle)
                ? VehiculeRegistry.ResolveName(p.DeviceId)
                : _pendingTitle;

            var session = new TripSession { DeviceId = p.DeviceId, StartTime = DateTime.UtcNow, Title = finalTitle };
            db.Sessions.Add(session);
            await db.SaveChangesAsync(ct);
            _currentSessionId = session.Id;
            logger.LogInformation("[DB] Nouvelle session {Id} créée ({Device}) : {Title}.", session.Id, p.DeviceId, _pendingTitle);
        }

        _batch.Add(TelemetryRecord.FromPayload(p, _currentSessionId.Value));

        if (_batch.Count >= BatchSize || DateTime.UtcNow - _lastSave >= FlushInterval)
            await FlushAsync(ct);
    }
    // persiste le batch en base et réinitialise l'état
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

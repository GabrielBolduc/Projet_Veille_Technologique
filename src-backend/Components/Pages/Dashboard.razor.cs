using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace AutoPi.TelemetryApi.Components.Pages;

public partial class Dashboard : IAsyncDisposable
{
    [Inject] NavigationManager Navigation { get; set; } = default!;
    [Inject] IDbContextFactory<TelemetryDbContext> DbFactory { get; set; } = default!;
    [Inject] DatabaseProcessor Processor { get; set; } = default!;
    [Inject] ITelemetryStreamer Streamer { get; set; } = default!;

    // SignalR

    private HubConnection? _hubConnection;

    private int    Rpm         = 0;
    private int    Speed       = 0;
    private double Throttle    = 0;
    private double EngineLoad  = 0;
    private double CoolantTemp = 0;
    private bool   DtcPresent  = false;
    private List<string> DtcCodes = new();

    private int MaxRpm   = 0;
    private int MaxSpeed = 0;

    private readonly List<int> RpmHistory   = new();
    private readonly List<int> SpeedHistory = new();
    private const int MaxHistory = 30;

    private bool IsShiftLightActive => Rpm >= 5500;

    private string RpmChartPoints   => HistoryToPoints(RpmHistory,   8000, 120, 24);
    private string SpeedChartPoints => HistoryToPoints(SpeedHistory,  220, 120, 24);

    private static string HistoryToPoints(List<int> history, double hardMax, double w, double h)
    {
        if (history.Count < 2) return string.Empty;
        double dataMax = history.Max();
        double scale = dataMax > 0 ? Math.Min(dataMax * 1.2, hardMax) : hardMax;
        double xStep = w / (history.Count - 1);
        return string.Join(" ", history.Select((v, i) =>
            $"{i * xStep:F1},{(1 - Math.Clamp(v / scale, 0, 1)) * h:F1}"));
    }

    // Onglets

    private string _activeTab = "live";

    private async Task SwitchTabAsync(string tab)
    {
        _activeTab = tab;
        if (tab == "history" && _sessions.Count == 0)
            await LoadHistoryAsync();
    }

    // Historique SQLite 

    private bool _loadingHistory = false;
    private List<SessionRow> _sessions = new();

    private sealed record SessionRow(
        int Id, string Title, string DeviceId, DateTime StartTime, DateTime? EndTime,
        int MaxRpm, int MaxSpeed, int RecordCount, double TotalKm,
        double AvgEngineLoad, double AvgCoolantTemp, double AvgThrottle)
    {
        public string Duration => EndTime.HasValue
            ? (EndTime.Value - StartTime).ToString(@"hh\:mm\:ss")
            : "En cours";
    }

    private string? _historyError;

    private async Task LoadHistoryAsync()
    {
        _loadingHistory = true;
        _historyError   = null;
        StateHasChanged();
        try
        {
            await using var db = await DbFactory.CreateDbContextAsync();
            var raw = await db.Sessions
                .OrderByDescending(s => s.StartTime)
                .Select(s => new
                {
                    s.Id, s.Title, s.DeviceId, s.StartTime, s.EndTime,
                    MaxRpm          = s.Records.Max(r => (int?)r.EngineRpm)              ?? 0,
                    MaxSpeed        = s.Records.Max(r => (int?)r.VehicleSpeed)           ?? 0,
                    RecordCount     = s.Records.Count(),
                    AvgEngineLoad   = s.Records.Average(r => (double?)r.EngineLoad)      ?? 0,
                    AvgCoolantTemp  = s.Records.Average(r => (double?)r.CoolantTemperature) ?? 0,
                    AvgThrottle     = s.Records.Average(r => (double?)r.ThrottlePosition) ?? 0
                })
                .ToListAsync();

            var sessionIds = raw.Select(x => x.Id).ToList();

            var speedPoints = await db.Records
                .Where(r => sessionIds.Contains(r.TripSessionId) && r.VehicleSpeed != null)
                .OrderBy(r => r.TripSessionId)
                .ThenBy(r => r.ReceivedAt)
                .Select(r => new { r.TripSessionId, r.ReceivedAt, r.VehicleSpeed })
                .ToListAsync();

            var kmBySession = speedPoints
                .GroupBy(r => r.TripSessionId)
                .ToDictionary(g => g.Key, g =>
                {
                    double km = 0;
                    var pts = g.ToList();
                    for (int i = 1; i < pts.Count; i++)
                    {
                        double hours    = (pts[i].ReceivedAt - pts[i - 1].ReceivedAt).TotalHours;
                        double avgSpeed = ((pts[i - 1].VehicleSpeed ?? 0) + (pts[i].VehicleSpeed ?? 0)) / 2.0;
                        km += avgSpeed * hours;
                    }
                    return km;
                });

            _sessions = raw.Select(x =>
                new SessionRow(x.Id, x.Title, x.DeviceId, x.StartTime, x.EndTime,
                               x.MaxRpm, x.MaxSpeed, x.RecordCount,
                               kmBySession.GetValueOrDefault(x.Id, 0.0),
                               x.AvgEngineLoad, x.AvgCoolantTemp, x.AvgThrottle))
                .ToList();
        }
        catch (Exception ex)
        {
            _historyError = ex.Message;
        }
        finally
        {
            _loadingHistory = false;
        }
    }

    // suivi trajet : démarrage, arrêt, reprise

    private bool   _showStartModal  = false;
    private string _newTripTitle    = "";
    private bool   _startModalError = false;

    private void OpenStartModal()
    {
        _newTripTitle    = "";
        _startModalError = false;
        _showStartModal  = true;
    }

    private void CloseStartModal()
    {
        _showStartModal  = false;
        _startModalError = false;
    }

    private async Task ConfirmStartTrackingAsync()
    {
        if (string.IsNullOrWhiteSpace(_newTripTitle))
        {
            _startModalError = true;
            return;
        }
        _showStartModal = false;
        await Processor.StartTrackingAsync(_newTripTitle.Trim());
        StateHasChanged();
    }

    private async Task OnStartModalKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")  await ConfirmStartTrackingAsync();
        if (e.Key == "Escape") CloseStartModal();
    }

    private bool _showStopModal = false;

    private void OpenStopModal()  => _showStopModal = true;
    private void CloseStopModal() => _showStopModal = false;

    private async Task ConfirmStopAsync()
    {
        _showStopModal = false;
        await Processor.CloseCurrentSessionAsync();
        await LoadHistoryAsync();
        StateHasChanged();
    }

    private async Task ResumeSessionAsync(int id)
    {
        await Processor.ResumeSessionAsync(id);
        await LoadHistoryAsync();
        StateHasChanged();
    }

    // Mode présentation

    private int? _demoSessionId = null;

    private async Task StartDemoAsync(int sessionId)
    {
        await Streamer.StartAsync(sessionId);
        _demoSessionId = sessionId;
        await SwitchTabAsync("live");
    }

    private async Task StopDemoAsync()
    {
        await Streamer.StopAsync();
        _demoSessionId = null;
        StateHasChanged();
    }

    // Suppression

    private bool   _showDeleteModal   = false;
    private int    _deleteTargetId    = 0;
    private string _deleteTargetTitle = "";

    private void OpenDeleteModal(int id, string title)
    {
        _deleteTargetId    = id;
        _deleteTargetTitle = title;
        _showDeleteModal   = true;
    }

    private void CloseDeleteModal()
    {
        _showDeleteModal = false;
    }

    private async Task ConfirmDeleteAsync()
    {
        _showDeleteModal = false;
        await Processor.DeleteSessionAsync(_deleteTargetId);
        await LoadHistoryAsync();
    }

    // Init & Dispose

    private async Task RestoreSessionMaxAsync()
    {
        try
        {
            await using var db = await DbFactory.CreateDbContextAsync();
            var active = await db.Sessions
                .Where(s => s.EndTime == null)
                .FirstOrDefaultAsync();

            if (active is not null)
            {
                MaxRpm = await db.Records
                    .Where(r => r.TripSessionId == active.Id)
                    .MaxAsync(r => (int?)r.EngineRpm) ?? 0;
                MaxSpeed = await db.Records
                    .Where(r => r.TripSessionId == active.Id)
                    .MaxAsync(r => (int?)r.VehicleSpeed) ?? 0;
            }
        }
        catch
        {
            // Non bloquant : les max seront mis à jour dès le prochain payload SignalR
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await RestoreSessionMaxAsync();

        var hubUrl = Navigation.ToAbsoluteUri("/telemetryHub").ToString();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<TelemetryPayload>("ReceiveTelemetry", payload =>
        {
            Rpm         = payload.Metrics.EngineRpm          ?? Rpm;
            Speed       = payload.Metrics.VehicleSpeed       ?? Speed;
            Throttle    = payload.Metrics.ThrottlePosition   ?? Throttle;
            EngineLoad  = payload.Metrics.EngineLoad         ?? EngineLoad;
            CoolantTemp = payload.Metrics.CoolantTemperature ?? CoolantTemp;
            DtcPresent  = payload.Diagnostics.DtcPresent;
            DtcCodes    = payload.Diagnostics.DtcCodes;

            if (payload.Metrics.EngineRpm.HasValue)
            {
                var rpm = payload.Metrics.EngineRpm.Value;
                if (rpm > MaxRpm) MaxRpm = rpm;
                RpmHistory.Add(rpm);
                if (RpmHistory.Count > MaxHistory) RpmHistory.RemoveAt(0);
            }
            if (payload.Metrics.VehicleSpeed.HasValue)
            {
                var spd = payload.Metrics.VehicleSpeed.Value;
                if (spd > MaxSpeed) MaxSpeed = spd;
                SpeedHistory.Add(spd);
                if (SpeedHistory.Count > MaxHistory) SpeedHistory.RemoveAt(0);
            }

            InvokeAsync(StateHasChanged);
        });

        await _hubConnection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
            await _hubConnection.DisposeAsync();
    }
}

using System.Text.Json;

namespace AutoPi.TelemetryApi;

public class TripSession
{
    public int       Id        { get; set; }
    public string    DeviceId  { get; set; } = "";
    public DateTime  StartTime { get; set; }
    public DateTime? EndTime   { get; set; }

    public List<TelemetryRecord> Records { get; set; } = new();
}

public class TelemetryRecord
{
    public long     Id            { get; set; }
    public int      TripSessionId { get; set; }
    public DateTime ReceivedAt    { get; set; }

    public string  DeviceId           { get; set; } = "";
    public string  Timestamp          { get; set; } = "";
    public int?    EngineRpm          { get; set; }
    public int?    VehicleSpeed       { get; set; }
    public double? ThrottlePosition   { get; set; }
    public double? EngineLoad         { get; set; }
    public double? CoolantTemperature { get; set; }
    public bool    DtcPresent         { get; set; }
    public string  DtcCodesJson       { get; set; } = "[]";

    public static TelemetryRecord FromPayload(TelemetryPayload p, int sessionId) => new()
    {
        TripSessionId      = sessionId,
        ReceivedAt         = DateTime.UtcNow,
        DeviceId           = p.DeviceId,
        Timestamp          = p.Timestamp,
        EngineRpm          = p.Metrics.EngineRpm,
        VehicleSpeed       = p.Metrics.VehicleSpeed,
        ThrottlePosition   = p.Metrics.ThrottlePosition,
        EngineLoad         = p.Metrics.EngineLoad,
        CoolantTemperature = p.Metrics.CoolantTemperature,
        DtcPresent         = p.Diagnostics.DtcPresent,
        DtcCodesJson       = JsonSerializer.Serialize(p.Diagnostics.DtcCodes)
    };
}

namespace AutoPi.TelemetryApi;

// model de donne pour EF Core pour les payloads
public record TelemetryPayload(
    string DeviceId,
    string Timestamp,
    TelemetryMetrics Metrics,
    TelemetryDiagnostics Diagnostics
);

// model de donne pour EF Core pour les sessions de conduite
public record TelemetryMetrics(
    int? EngineRpm,
    int? VehicleSpeed,
    double? ThrottlePosition,
    double? EngineLoad,
    double? CoolantTemperature
);

// model de donne pour EF Core pour les diagnostics
public record TelemetryDiagnostics(
    bool DtcPresent,
    List<string> DtcCodes
);

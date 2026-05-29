namespace AutoPi.TelemetryApi;

public record TelemetryPayload(
    string DeviceId,
    string Timestamp,
    TelemetryMetrics Metrics,
    TelemetryDiagnostics Diagnostics
);

public record TelemetryMetrics(
    int? EngineRpm,
    int? VehicleSpeed,
    double? ThrottlePosition,
    double? EngineLoad,
    double? CoolantTemperature
);

public record TelemetryDiagnostics(
    bool DtcPresent,
    List<string> DtcCodes
);

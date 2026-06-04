namespace AutoPi.TelemetryApi;

public interface ITelemetryStreamer
{
    bool IsStreaming { get; }
    Task StartAsync(int? sessionId = null);
    Task StopAsync();
}

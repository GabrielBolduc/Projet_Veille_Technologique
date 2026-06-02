using System.Threading.Channels;

namespace AutoPi.TelemetryApi;

// Wrappers typés pour deux canaux distincts dans le conteneur DI
public sealed class DbChannelWriter(ChannelWriter<TelemetryPayload> inner)
{
    public ValueTask WriteAsync(TelemetryPayload item, CancellationToken ct = default)
        => inner.WriteAsync(item, ct);

    public bool TryWrite(TelemetryPayload item) => inner.TryWrite(item);
}

public sealed class DbChannelReader(ChannelReader<TelemetryPayload> inner)
{
    public IAsyncEnumerable<TelemetryPayload> ReadAllAsync(CancellationToken ct = default)
        => inner.ReadAllAsync(ct);
}

// Stats pour la session
public class SessionStats
{
    public int MaxRpm   { get; private set; }
    public int MaxSpeed { get; private set; }

    public void UpdateRpm(int rpm)     { if (rpm   > MaxRpm)   MaxRpm   = rpm;   }
    public void UpdateSpeed(int speed) { if (speed > MaxSpeed) MaxSpeed = speed; }
    public void Reset()                { MaxRpm = 0; MaxSpeed = 0; }
}

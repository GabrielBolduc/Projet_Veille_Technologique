using Microsoft.AspNetCore.SignalR;

namespace AutoPi.TelemetryApi;

public class TelemetryHub(ILogger<TelemetryHub> logger) : Hub
{
    public override Task OnConnectedAsync()
    {
        logger.LogInformation("Client SignalR connecté : {ConnectionId}", Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is null)
            logger.LogInformation("Client SignalR déconnecté : {ConnectionId}", Context.ConnectionId);
        else
            logger.LogWarning(exception,
                "Client SignalR déconnecté avec erreur : {ConnectionId}", Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}

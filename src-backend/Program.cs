using System.Threading.Channels;
using AutoPi.TelemetryApi;
using AutoPi.TelemetryApi.Components;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5000");

var channel = Channel.CreateUnbounded<TelemetryPayload>(new UnboundedChannelOptions
{
    SingleReader = true
});

builder.Services.AddSingleton(channel.Reader);
builder.Services.AddSingleton(channel.Writer);
builder.Services.AddHostedService<TelemetryProcessor>();

builder.Services.AddSignalR();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
});

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapPost("/api/telemetry", async (TelemetryPayload payload, ChannelWriter<TelemetryPayload> writer) =>
{
    await writer.WriteAsync(payload);
    return Results.Accepted();
});

app.MapHub<TelemetryHub>("/telemetryHub");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

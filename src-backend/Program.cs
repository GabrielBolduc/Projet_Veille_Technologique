using System.Threading.Channels;
using AutoPi.TelemetryApi;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5000");

var channel = Channel.CreateUnbounded<TelemetryPayload>(new UnboundedChannelOptions
{
    SingleReader = true
});

builder.Services.AddSingleton(channel.Reader);
builder.Services.AddSingleton(channel.Writer);
builder.Services.AddHostedService<TelemetryProcessor>();

var app = builder.Build();

app.MapPost("/api/telemetry", async (TelemetryPayload payload, ChannelWriter<TelemetryPayload> writer) =>
{
    await writer.WriteAsync(payload);
    return Results.Accepted();
});
builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
});

app.Run();

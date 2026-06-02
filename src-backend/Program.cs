using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using AutoPi.TelemetryApi;
using AutoPi.TelemetryApi.Components;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5000");

// canal 1 pour affichage Blazor
var uiChannel = Channel.CreateUnbounded<TelemetryPayload>(new UnboundedChannelOptions
{
    SingleReader = true // Seul TelemetryProcessor va consommer ce canal
});
builder.Services.AddSingleton(uiChannel.Reader);
builder.Services.AddSingleton(uiChannel.Writer);

// canal 2 pour écriture en base de données
var dbChannel = Channel.CreateUnbounded<TelemetryPayload>(new UnboundedChannelOptions
{
    SingleReader = true // Seul DatabaseProcessor va consommer ce wrapper
});
builder.Services.AddSingleton(new DbChannelWriter(dbChannel.Writer));
builder.Services.AddSingleton(new DbChannelReader(dbChannel.Reader));

builder.Services.AddHostedService<TelemetryProcessor>();

// Configuration de la base de données SQLite avec EF Core
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "autopi.db");
builder.Services.AddDbContext<TelemetryDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddSingleton<SessionStats>();
builder.Services.AddHostedService<DatabaseProcessor>();

builder.Services.AddSignalR();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
});

var app = builder.Build();

try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
    await db.Database.EnsureCreatedAsync();
    app.Logger.LogInformation("[DB] autopi.db initialisée : {Path}", dbPath);
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "[DB] Impossible d'initialiser autopi.db à {Path}. Arrêt.", dbPath);
    throw;
}

app.UseExceptionHandler(errPipeline =>
{
    errPipeline.Run(async ctx =>
    {
        var feature = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var log = ctx.RequestServices.GetRequiredService<ILoggerFactory>()
                     .CreateLogger("GlobalExceptionHandler");
        if (feature?.Error is not null)
            log.LogError(feature.Error, "Exception non gérée sur {Method} {Path}",
                ctx.Request.Method, ctx.Request.Path);
        ctx.Response.StatusCode  = StatusCodes.Status500InternalServerError;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsync("{\"error\":\"Erreur interne du serveur.\"}");
    });
});

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
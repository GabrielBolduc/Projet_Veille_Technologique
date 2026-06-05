using Microsoft.EntityFrameworkCore;

namespace AutoPi.TelemetryApi;

public class TelemetryDbContext : DbContext
{
    public DbSet<TripSession> Sessions => Set<TripSession>();
    public DbSet<TelemetryRecord> Records => Set<TelemetryRecord>();

    public TelemetryDbContext(DbContextOptions<TelemetryDbContext> options) : base(options)
    {
    }

    // Configuration du modèle de données
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelemetryRecord>()
            .HasIndex(r => r.Timestamp);

        modelBuilder.Entity<TelemetryRecord>()
            .HasOne<TripSession>()
            .WithMany(s => s.Records)
            .HasForeignKey(r => r.TripSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
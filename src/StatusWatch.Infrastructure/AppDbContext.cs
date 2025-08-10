using Microsoft.EntityFrameworkCore;
using StatusWatch.Domain.Entities;

namespace StatusWatch.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Probe> Probes => Set<Probe>();
    public DbSet<ProbeResult> ProbeResults => Set<ProbeResult>();
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<IncidentEvent> IncidentEvents => Set<IncidentEvent>();
    public DbSet<ProbeStatus> ProbeStatuses => Set<ProbeStatus>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Project>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.HasIndex(x => x.Name).IsUnique();
        });

        b.Entity<Service>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.HasOne(x => x.Project).WithMany(p => p.Services).HasForeignKey(x => x.ProjectId);
            e.HasIndex(x => new { x.ProjectId, x.Name }).IsUnique();
        });

        b.Entity<Probe>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Target).IsRequired().HasMaxLength(500);
            e.HasOne(x => x.Service).WithMany(s => s.Probes).HasForeignKey(x => x.ServiceId);
            e.HasIndex(x => x.NextRunAtUtc);
        });

        b.Entity<ProbeResult>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Probe).WithMany(p => p.Results).HasForeignKey(x => x.ProbeId);
            e.HasIndex(x => new { x.ProbeId, x.Timestamp });
        });

        b.Entity<ProbeStatus>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Probe)
             .WithMany()             
             .HasForeignKey(x => x.ProbeId);
            e.HasIndex(x => x.ProbeId).IsUnique();
        });

        b.Entity<Incident>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Service).WithMany().HasForeignKey(x => x.ServiceId);
            e.HasIndex(x => new { x.ServiceId, x.Status });
        });

        b.Entity<IncidentEvent>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Incident).WithMany(i => i.Events).HasForeignKey(x => x.IncidentId);
            e.HasIndex(x => new { x.IncidentId, x.TimestampUtc });
        });


    }
}

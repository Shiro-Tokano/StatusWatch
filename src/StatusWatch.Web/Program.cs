using Microsoft.EntityFrameworkCore;
using StatusWatch.Infrastructure;
using StatusWatch.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddInfrastructure(builder.Configuration);

// healthcheck БД
builder.Services.AddHealthChecks().AddNpgSql(
    builder.Configuration.GetConnectionString("DefaultConnection") ??
    Environment.GetEnvironmentVariable("POSTGRES_CONNECTION"));

var app = builder.Build();

/*if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}*/

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapHealthChecks("/health/db");

// -------- Projects --------
app.MapGet("/api/projects", async (AppDbContext db) => await db.Projects.ToListAsync());

app.MapPost("/api/projects", async (AppDbContext db, ProjectDto dto) =>
{
    var p = new Project { Name = dto.Name };
    db.Projects.Add(p);
    await db.SaveChangesAsync();
    return Results.Created($"/api/projects/{p.Id}", p);
});

// -------- Services & Probes --------
app.MapGet("/api/services/{serviceId:int}/probes", async (AppDbContext db, int serviceId) =>
    await db.Probes.Where(p => p.ServiceId == serviceId).ToListAsync());

app.MapPost("/api/services", async (AppDbContext db, CreateServiceDto dto) =>
{
    var s = new Service { ProjectId = dto.ProjectId, Name = dto.Name, Url = dto.Url };
    db.Services.Add(s);
    await db.SaveChangesAsync();
    return Results.Created($"/api/services/{s.Id}", s);
});

app.MapPost("/api/probes", async (AppDbContext db, CreateProbeDto dto) =>
{
    var p = new Probe
    {
        ServiceId = dto.ServiceId,
        Type = dto.Type,
        Target = dto.Target,
        Interval = TimeSpan.FromSeconds(dto.IntervalSeconds),
        IsEnabled = true,
        NextRunAtUtc = DateTime.UtcNow
    };
    db.Probes.Add(p);
    await db.SaveChangesAsync();
    return Results.Created($"/api/probes/{p.Id}", p);
});

app.MapGet("/api/probes/{id:int}/results", async (AppDbContext db, int id, int take = 50) =>
    await db.ProbeResults.Where(r => r.ProbeId == id)
        .OrderByDescending(r => r.Timestamp)
        .Take(take)
        .ToListAsync());

// открытые инциденты
app.MapGet("/api/incidents/open", async (AppDbContext db) =>
    await db.Incidents.Where(i => i.Status == IncidentStatus.Open)
        .OrderByDescending(i => i.OpenedAtUtc)
        .ToListAsync());

// инциденты по сервису
app.MapGet("/api/services/{serviceId:int}/incidents", async (AppDbContext db, int serviceId) =>
    await db.Incidents.Where(i => i.ServiceId == serviceId)
        .OrderByDescending(i => i.OpenedAtUtc)
        .Take(50)
        .ToListAsync());

// таймлайн инцидента
app.MapGet("/api/incidents/{id:int}/events", async (AppDbContext db, int id) =>
    await db.IncidentEvents.Where(e => e.IncidentId == id)
        .OrderBy(e => e.TimestampUtc)
        .ToListAsync());

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// DTO records
record ProjectDto(string Name);
record CreateServiceDto(int ProjectId, string Name, string? Url);
record CreateProbeDto(int ServiceId, ProbeType Type, string Target, int IntervalSeconds);

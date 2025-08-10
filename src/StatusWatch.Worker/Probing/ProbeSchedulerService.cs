using Microsoft.EntityFrameworkCore;
using StatusWatch.Domain.Entities;
using StatusWatch.Infrastructure;

namespace StatusWatch.Worker;

public class ProbeSchedulerService : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly Dictionary<ProbeType, IProbeExecutor> _executors;

    public ProbeSchedulerService(IServiceProvider sp, IEnumerable<IProbeExecutor> executors)
    {
        _sp = sp;
        _executors = executors.ToDictionary(x => x.Type, x => x);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // тикер планировщика
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await TickAsync(stoppingToken);
            }
            catch { /* позже крч добавлю */ }

            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task TickAsync(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;

        // Берём до 50 задач к запуску
        var due = await db.Probes
            .Where(p => p.IsEnabled && (p.NextRunAtUtc == null || p.NextRunAtUtc <= now))
            .OrderBy(p => p.NextRunAtUtc ?? DateTime.MinValue)
            .Take(50)
            .ToListAsync(ct);

        if (due.Count == 0) return;

        // Резервируем слоты запуска, чтобы другой экземпляр не схватил
        foreach (var p in due)
            p.NextRunAtUtc = now + p.Interval;

        await db.SaveChangesAsync(ct);

        // Исполняем параллельно
        var tasks = due.Select(p => ExecuteOneAsync(p, ct));
        await Task.WhenAll(tasks);
    }

    private async Task ExecuteOneAsync(Probe probe, CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!_executors.TryGetValue(probe.Type, out var exec)) return;

        var result = await exec.ExecuteAsync(probe, ct);
        db.ProbeResults.Add(result);
        await db.SaveChangesAsync(ct); // lol
        await IncidentLogic.HandleResultAsync(db, probe, result, ct); // lol
        // Обновим NextRunAtUtc, если интервал изменился или была ошибка с большим временем
        await db.SaveChangesAsync(ct);
    }
}

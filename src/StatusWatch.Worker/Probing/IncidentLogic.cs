using Microsoft.EntityFrameworkCore;
using StatusWatch.Domain.Entities;
using StatusWatch.Infrastructure;

namespace StatusWatch.Worker;

public static class IncidentLogic
{
    private const int OpenAfterFails = 3;
    private const int CloseAfterSuccesses = 2;

    public static async Task HandleResultAsync(AppDbContext db, Probe probe, ProbeResult result, CancellationToken ct)
    {
        // обновляем стрики
        var st = await db.ProbeStatuses.SingleOrDefaultAsync(x => x.ProbeId == probe.Id, ct);
        if (st == null)
        {
            st = new ProbeStatus { ProbeId = probe.Id };
            db.ProbeStatuses.Add(st);
        }

        if (result.IsSuccess)
        {
            st.SuccessStreak = (st.LastIsSuccess == true ? st.SuccessStreak : 0) + 1;
            st.FailStreak = 0;
            st.LastIsSuccess = true;
        }
        else
        {
            st.FailStreak = (st.LastIsSuccess == false ? st.FailStreak : 0) + 1;
            st.SuccessStreak = 0;
            st.LastIsSuccess = false;
        }
        st.LastAtUtc = result.Timestamp;

        // состояние сервиса по агрегату «любой пробе плохо»
        if (!result.IsSuccess && st.FailStreak == OpenAfterFails)
        {
            // открыть инцидент, если нет открытого
            var hasOpen = await db.Incidents.AnyAsync(i => i.ServiceId == probe.ServiceId && i.Status == IncidentStatus.Open, ct);
            if (!hasOpen)
            {
                var inc = new Incident
                {
                    ServiceId = probe.ServiceId,
                    Status = IncidentStatus.Open,
                    OpenedAtUtc = DateTime.UtcNow,
                    Title = "Деградация сервиса",
                    OpenReason = $"Порог: {OpenAfterFails} подряд фейлов проб."
                };
                inc.Events.Add(new IncidentEvent { Message = $"Открыт: ProbeId={probe.Id}, ошибка: {result.Error ?? (result.StatusCode?.ToString() ?? "fail")}" });
                db.Incidents.Add(inc);
            }
        }
        else if (result.IsSuccess && st.SuccessStreak == CloseAfterSuccesses)
        {
            // закрыть открытый
            var inc = await db.Incidents
                .Where(i => i.ServiceId == probe.ServiceId && i.Status == IncidentStatus.Open)
                .OrderByDescending(i => i.OpenedAtUtc)
                .FirstOrDefaultAsync(ct);

            if (inc != null)
            {
                inc.Status = IncidentStatus.Resolved;
                inc.ResolvedAtUtc = DateTime.UtcNow;
                inc.ResolveReason = $"Порог: {CloseAfterSuccesses} подряд успехов проб.";
                db.IncidentEvents.Add(new IncidentEvent { IncidentId = inc.Id, Message = $"Закрыт: ProbeId={probe.Id} восстановился." });
            }
        }
    }
}

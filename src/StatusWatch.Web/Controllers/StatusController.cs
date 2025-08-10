using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatusWatch.Infrastructure;
using StatusWatch.Web.Models;
using StatusWatch.Domain.Entities;

namespace StatusWatch.Web.Controllers;

public class StatusController(AppDbContext db) : Controller
{
    // HTML страница
    [HttpGet("/status")]
    public async Task<IActionResult> Index()
    {
        var list = await QueryStatuses(db).ToListAsync();
        return View(list);
    }

    // JSON сводка
    [HttpGet("/api/status/summary")]
    public async Task<ActionResult<IEnumerable<ServiceStatusVm>>> Summary()
        => Ok(await QueryStatuses(db).ToListAsync());

    // общий запрос
    private static IQueryable<ServiceStatusVm> QueryStatuses(AppDbContext db)
        => db.Services.Select(s => new ServiceStatusVm(
            s.Id,
            s.Name,
            s.Url,
            db.Incidents.Any(i => i.ServiceId == s.Id && i.Status == IncidentStatus.Open),
            db.ProbeResults.Where(r => r.Probe.ServiceId == s.Id)
                           .OrderByDescending(r => r.Timestamp)
                           .Select(r => (DateTime?)r.Timestamp)
                           .FirstOrDefault(),
            db.ProbeResults.Where(r => r.Probe.ServiceId == s.Id)
                           .OrderByDescending(r => r.Timestamp)
                           .Select(r => r.LatencyMs)
                           .FirstOrDefault(),
            db.ProbeResults.Where(r => r.Probe.ServiceId == s.Id)
                           .OrderByDescending(r => r.Timestamp)
                           .Select(r => (bool?)r.IsSuccess)
                           .FirstOrDefault()
        ));
}

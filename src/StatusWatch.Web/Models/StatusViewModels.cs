namespace StatusWatch.Web.Models;

public record ServiceStatusVm(
    int ServiceId,
    string Name,
    string? Url,
    bool HasOpenIncident,
    DateTime? LastCheckUtc,
    int? LastLatencyMs,
    bool? LastIsSuccess
);

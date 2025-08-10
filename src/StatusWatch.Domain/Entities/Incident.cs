namespace StatusWatch.Domain.Entities;
public class Incident
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public DateTime OpenedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAtUtc { get; set; }
    public string Title { get; set; } = "Деградация сервиса";
    public string? OpenReason { get; set; }
    public string? ResolveReason { get; set; }

    public Service Service { get; set; } = null!;
    public ICollection<IncidentEvent> Events { get; set; } = new List<IncidentEvent>();
}

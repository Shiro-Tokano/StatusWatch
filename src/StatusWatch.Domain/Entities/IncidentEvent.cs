namespace StatusWatch.Domain.Entities;
public class IncidentEvent
{
    public long Id { get; set; }
    public int IncidentId { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public string Message { get; set; } = "";
    public Incident Incident { get; set; } = null!;
}

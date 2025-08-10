namespace StatusWatch.Domain.Entities;
public class ProbeResult
{
    public long Id { get; set; }
    public int ProbeId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsSuccess { get; set; }
    public int? StatusCode { get; set; }
    public int? LatencyMs { get; set; }
    public string? Error { get; set; }

    public Probe Probe { get; set; } = null!;
}

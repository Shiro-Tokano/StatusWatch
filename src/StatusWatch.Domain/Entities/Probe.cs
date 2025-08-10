namespace StatusWatch.Domain.Entities;
public class Probe
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public ProbeType Type { get; set; }
    public string Target { get; set; } = "";
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(1);
    public bool IsEnabled { get; set; } = true;
    public Service Service { get; set; } = null!;
    public DateTime? NextRunAtUtc { get; set; } = null;
    public ICollection<ProbeResult> Results { get; set; } = new List<ProbeResult>();
}

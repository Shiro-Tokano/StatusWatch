namespace StatusWatch.Domain.Entities;
public class ProbeStatus
{
    public int Id { get; set; }
    public int ProbeId { get; set; }
    public int FailStreak { get; set; }
    public int SuccessStreak { get; set; }
    public bool? LastIsSuccess { get; set; }
    public DateTime? LastAtUtc { get; set; }
    public Probe Probe { get; set; } = null!;
}

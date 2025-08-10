namespace StatusWatch.Domain.Entities;
public class Service
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Name { get; set; } = "";
    public string? Url { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Project Project { get; set; } = null!;
    public ICollection<Probe> Probes { get; set; } = new List<Probe>();
}

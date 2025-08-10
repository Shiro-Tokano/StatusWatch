namespace StatusWatch.Domain.Entities;
public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Service> Services { get; set; } = new List<Service>();
}

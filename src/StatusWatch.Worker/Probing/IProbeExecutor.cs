using StatusWatch.Domain.Entities;

namespace StatusWatch.Worker;

public interface IProbeExecutor
{
    ProbeType Type { get; }
    Task<ProbeResult> ExecuteAsync(Probe probe, CancellationToken ct);
}

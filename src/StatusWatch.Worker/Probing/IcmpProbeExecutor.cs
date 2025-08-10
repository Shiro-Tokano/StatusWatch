using System.Diagnostics;
using System.Net.NetworkInformation;
using StatusWatch.Domain.Entities;

namespace StatusWatch.Worker;

public class IcmpProbeExecutor : IProbeExecutor
{
    public ProbeType Type => ProbeType.Icmp;

    public async Task<ProbeResult> ExecuteAsync(Probe probe, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(probe.Target, 5000);
            sw.Stop();

            return new ProbeResult
            {
                ProbeId = probe.Id,
                Timestamp = DateTime.UtcNow,
                IsSuccess = reply.Status == IPStatus.Success,
                StatusCode = null,
                LatencyMs = reply.Status == IPStatus.Success ? (int?)reply.RoundtripTime : (int?)sw.ElapsedMilliseconds,
                Error = reply.Status == IPStatus.Success ? null : reply.Status.ToString()
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new ProbeResult
            {
                ProbeId = probe.Id,
                Timestamp = DateTime.UtcNow,
                IsSuccess = false,
                Error = ex.Message,
                LatencyMs = (int)sw.ElapsedMilliseconds
            };
        }
    }
}

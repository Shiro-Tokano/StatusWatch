using System.Diagnostics;
using System.Net.Sockets;
using StatusWatch.Domain.Entities;

namespace StatusWatch.Worker;

public class TcpProbeExecutor : IProbeExecutor
{
    public ProbeType Type => ProbeType.Tcp;

    public async Task<ProbeResult> ExecuteAsync(Probe probe, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            // Target вида "host:port"
            var parts = probe.Target.Split(':', 2);
            var host = parts[0];
            var port = int.Parse(parts[1]);

            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(host, port);
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            var completed = await Task.WhenAny(connectTask, Task.Delay(Timeout.Infinite, cts.Token));
            if (completed != connectTask)
                throw new TimeoutException("TCP connect timeout");

            sw.Stop();
            return new ProbeResult
            {
                ProbeId = probe.Id,
                Timestamp = DateTime.UtcNow,
                IsSuccess = client.Connected,
                StatusCode = null,
                LatencyMs = (int)sw.ElapsedMilliseconds
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

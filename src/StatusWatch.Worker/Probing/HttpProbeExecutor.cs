using System.Diagnostics;
using System.Net.Http;
using StatusWatch.Domain.Entities;

namespace StatusWatch.Worker;

public class HttpProbeExecutor(HttpClient http) : IProbeExecutor
{
    public ProbeType Type => ProbeType.Http;

    public async Task<ProbeResult> ExecuteAsync(Probe probe, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, probe.Target);
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            var resp = await http.SendAsync(req, cts.Token);
            sw.Stop();

            return new ProbeResult
            {
                ProbeId = probe.Id,
                Timestamp = DateTime.UtcNow,
                IsSuccess = resp.IsSuccessStatusCode,
                StatusCode = (int)resp.StatusCode,
                LatencyMs = (int)sw.ElapsedMilliseconds,
                Error = resp.IsSuccessStatusCode ? null : $"HTTP {(int)resp.StatusCode}"
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
                StatusCode = null,
                LatencyMs = (int)sw.ElapsedMilliseconds,
                Error = ex.Message
            };
        }
    }
}

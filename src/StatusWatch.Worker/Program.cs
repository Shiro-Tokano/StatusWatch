using Microsoft.EntityFrameworkCore;
using StatusWatch.Infrastructure;
using StatusWatch.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

// HttpClient для HTTP?проб
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IProbeExecutor, HttpProbeExecutor>();
builder.Services.AddSingleton<IProbeExecutor, TcpProbeExecutor>();
builder.Services.AddSingleton<IProbeExecutor, IcmpProbeExecutor>();
builder.Services.AddHostedService<ProbeSchedulerService>();

var host = builder.Build();
await host.RunAsync();

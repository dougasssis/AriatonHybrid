using Ariston.Thermos;
using Microsoft.Extensions.Hosting;

namespace Ariston.Services;

public class Worker(Thermo _thermo) : BackgroundService
{ 
    private async Task MonitorAndControl(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _thermo.Heartbeat();
            await Task.Delay(delay: TimeSpan.FromMinutes(5), cancellationToken: stoppingToken);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await MonitorAndControl(stoppingToken: stoppingToken);
    }
}
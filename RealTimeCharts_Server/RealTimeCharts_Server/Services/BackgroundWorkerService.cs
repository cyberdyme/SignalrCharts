using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RealTimeCharts_Server.DataStorage;
using RealTimeCharts_Server.HubConfig;

namespace RealTimeCharts_Server.Services
{
    public interface IBackgroundWorkerServiceOptions
    {
        bool SendMessages { get; set; }
    }


    public class BackgroundWorkerServiceOptions: IBackgroundWorkerServiceOptions
    {
        public bool SendMessages { get; set; }
    }


    public class BackgroundWorkerService : BackgroundService
    {
        private readonly ILogger<BackgroundWorkerService> _logger;
        private readonly IHubContext<ChartHub, IChartHub> _hubContext;
        private readonly IBackgroundWorkerServiceOptions _options;

        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IHubContext<ChartHub, IChartHub> hubContext, IBackgroundWorkerServiceOptions options)
        {
            _logger = logger;
            _hubContext = hubContext;
            _options = options;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return this.RunTask(stoppingToken);
        }

        private async Task<int> RunTask(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                _logger.LogInformation("Processing service worker ....");
                if (_options.SendMessages)
                {
                    await _hubContext.Clients.All.BroadcastChartData(DataManager.GetData());
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            return 1;
        }

    }
}

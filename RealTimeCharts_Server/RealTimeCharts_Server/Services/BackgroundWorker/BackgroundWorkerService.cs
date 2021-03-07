using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RealTimeCharts_Server.DataStorage;
using RealTimeCharts_Server.HubConfig;

namespace RealTimeCharts_Server.Services.BackgroundWorker
{
    public class BackgroundWorkerService : BackgroundService
    {
        private readonly IHubContext<ChartHub, IChartHub> _hubContext;
        private readonly ILogger<BackgroundWorkerService> _logger;
        private readonly IBackgroundWorkerServiceOptions _options;


        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger,
            IHubContext<ChartHub, IChartHub> hubContext, IBackgroundWorkerServiceOptions options)
        {
            _logger = logger;
            _hubContext = hubContext;
            _options = options;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("***************** Starting the task ******************");
            return RunTask(stoppingToken);
        }

        private async Task<int> RunTask(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DataManager.GetData getting data ...");
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_options.SendMessages)
                {
                    _logger.LogInformation(".");
                    await _hubContext.Clients.All.BroadcastChartData(DataManager.GetData());
                }

                await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
            }

            _logger.LogInformation("***************** BackgroundWorkerService completed ******************");
            return 1;
        }
    }
}
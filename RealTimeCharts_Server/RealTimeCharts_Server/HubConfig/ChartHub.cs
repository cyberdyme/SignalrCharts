using Microsoft.AspNetCore.SignalR;
using RealTimeCharts_Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealTimeCharts_Server.Services;
using RealTimeCharts_Server.Services.BackgroundWorker;

namespace RealTimeCharts_Server.HubConfig
{
    public class ChartHub : Hub<IChartHub>
    {
        private readonly IServiceNotificationService _notificationService;

        public ChartHub(IServiceNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task BroadcastChartData(List<ChartModel> data) => await Clients.All.BroadcastChartData(data);

        public Task StartBroadcastChartData()
        {
            _notificationService.SetMessageStatus(true);
            return Task.FromResult(true);
        }

        public Task EndBroadcastChartData()
        {
            _notificationService.SetMessageStatus(false);
            return Task.FromResult(true);
        }
    }

    public interface IChartHub
    {
        Task BroadcastChartData(List<ChartModel> data);

        Task StartBroadcastChartData();


        Task EndBroadcastChartData();
    }
}

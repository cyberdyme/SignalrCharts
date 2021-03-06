﻿using Microsoft.AspNetCore.SignalR;
using RealTimeCharts_Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealTimeCharts_Server.Services;

namespace RealTimeCharts_Server.HubConfig
{
    public class ChartHub : Hub<IChartHub>
    {
        private readonly IBackgroundWorkerServiceOptions _options;

        public ChartHub(IBackgroundWorkerServiceOptions options)
        {
            _options = options;
        }

        public async Task BroadcastChartData(List<ChartModel> data) => await Clients.All.BroadcastChartData(data);

        public Task StartBroadcastChartData()
        {
            _options.SendMessages = true;
            return Task.FromResult(true);
        }

        public Task EndBroadcastChartData()
        {
            _options.SendMessages = false;
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

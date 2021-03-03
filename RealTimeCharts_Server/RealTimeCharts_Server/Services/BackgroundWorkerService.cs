using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RealTimeCharts_Server.DataStorage;
using RealTimeCharts_Server.HubConfig;

namespace RealTimeCharts_Server.Services
{
    public class ServiceNotificationOptions : INotification
    {
        public bool SendMessages { get; set; }
    }


    public interface IServiceNotificationService
    {
        void SetMessageStatus(bool sendMessageStatus);
    }

    public class ServiceNotificationService : IServiceNotificationService
    {
        private readonly IMediator _mediator;

        public ServiceNotificationService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void SetMessageStatus(bool sendMessageStatus)
        {
            _mediator.Publish(new ServiceNotificationOptions { SendMessages = sendMessageStatus });
        }
    }



    public interface IBackgroundWorkerServiceOptions
    {
        bool SendMessages { get; set; }
    }


    public class BackgroundWorkerServiceOptions: IBackgroundWorkerServiceOptions
    {
        public bool SendMessages { get; set; }
    }

    public interface IBackgroundWorkerService : IHostedService
    {
    }


    public class BackgroundWorkerService : BackgroundService, INotificationHandler<ServiceNotificationOptions>, IBackgroundWorkerService
    {
        private readonly ILogger<BackgroundWorkerService> _logger;
        private readonly IHubContext<ChartHub, IChartHub> _hubContext;

        static readonly ReaderWriterLockSlim ReaderWriterLockSlim = new();
        private static bool _sendMessages;

        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IHubContext<ChartHub, IChartHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("***************** Starting the task ******************");
            return this.RunTask(stoppingToken);
        }

        private async Task<int> RunTask(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DataManager.GetData getting data ...");
            while (!stoppingToken.IsCancellationRequested)
            {
                // read value
                bool sendMessagesStatus;
                //ReaderWriterLockSlim.EnterReadLock();
                try
                {
                    sendMessagesStatus = BackgroundWorkerService._sendMessages;
                }
                finally
                {
                    //ReaderWriterLockSlim.ExitReadLock();
                }

                
                if (sendMessagesStatus)
                {
                    _logger.LogInformation(".");
                    await _hubContext.Clients.All.BroadcastChartData(DataManager.GetData());

                }
                await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
            }

            _logger.LogInformation("***************** BackgroundWorkerService completed ******************");
            return 1;
        }

        public Task Handle(ServiceNotificationOptions notification, CancellationToken cancellationToken)
        {
            // write the value
            //ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                BackgroundWorkerService._sendMessages = notification.SendMessages;
            }
            finally
            {
                //ReaderWriterLockSlim.ExitWriteLock();
            }


            _logger.LogCritical($"Debugging from Notifier 1. Message  : {notification.SendMessages} ");
            return Task.CompletedTask;
        }
    }
}

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


    public class BackgroundWorkerServiceOptions: IBackgroundWorkerServiceOptions, INotificationHandler<ServiceNotificationOptions> {
        private readonly ReaderWriterLockSlim _readerWriterLockSlim = new();
        private bool _sendMessages;

        public BackgroundWorkerServiceOptions()
        {
            
        }


        public bool SendMessages
        {
            get
            {
                _readerWriterLockSlim.EnterReadLock();
                try
                {
                    return _sendMessages;
                }
                finally
                {
                    _readerWriterLockSlim.ExitReadLock();
                }
            }

            set
            {
                
                _readerWriterLockSlim.EnterWriteLock();
                try
                {
                    _sendMessages = value;
                }
                finally
                {
                    _readerWriterLockSlim.ExitWriteLock();
                }
            }
        }


        public Task Handle(ServiceNotificationOptions notification, CancellationToken cancellationToken)
        {
            this.SendMessages = notification.SendMessages;
            return Task.CompletedTask;
        }
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
            _logger.LogInformation("***************** Starting the task ******************");
            return this.RunTask(stoppingToken);
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

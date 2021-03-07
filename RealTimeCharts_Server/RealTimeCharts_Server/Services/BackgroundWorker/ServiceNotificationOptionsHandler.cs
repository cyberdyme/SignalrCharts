using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace RealTimeCharts_Server.Services.BackgroundWorker
{
    // ReSharper disable once UnusedMember.Global
    public class ServiceNotificationOptionsHandler : INotificationHandler<ServiceNotificationOptions>
    {
        private readonly IBackgroundWorkerServiceOptions _options;

        public ServiceNotificationOptionsHandler()
        {
            
        }


        public ServiceNotificationOptionsHandler(IBackgroundWorkerServiceOptions options)
        {
            _options = options;
        }

        public Task Handle(ServiceNotificationOptions notification, CancellationToken cancellationToken)
        {
            _options.SendMessages = notification.SendMessages;
            return Task.CompletedTask;
        }
    }
}
using MediatR;

namespace RealTimeCharts_Server.Services.BackgroundWorker
{
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
            _mediator.Publish(new ServiceNotificationOptions {SendMessages = sendMessageStatus});
        }
    }
}
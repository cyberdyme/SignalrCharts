using MediatR;

namespace RealTimeCharts_Server.Services.BackgroundWorker
{
    public class ServiceNotificationOptions : INotification
    {
        public bool SendMessages { get; set; }
    }
}
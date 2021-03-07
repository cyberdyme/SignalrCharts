using Autofac;
using RealTimeCharts_Server.Services;
using RealTimeCharts_Server.Services.BackgroundWorker;

namespace RealTimeCharts_Server.IOC
{
    public class ServiceRegistration
    {
        public static void Register(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceNotificationService>().As<IServiceNotificationService>().InstancePerDependency();
        }

        public static void RegisterBackgroundServices(ContainerBuilder builder)
        {
            builder.RegisterType<BackgroundWorkerServiceOptions>().As<IBackgroundWorkerServiceOptions>().SingleInstance();
        }
    }
}

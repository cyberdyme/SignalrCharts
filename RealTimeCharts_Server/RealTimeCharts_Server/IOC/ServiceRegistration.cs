using Autofac;
using RealTimeCharts_Server.Services;

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
        }
    }
}

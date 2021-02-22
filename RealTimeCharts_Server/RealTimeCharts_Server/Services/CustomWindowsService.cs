using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RealTimeCharts_Server.Services
{
    // https://github.com/dotnet/extensions/issues/2568
    public class CustomWindowsService : WindowsServiceLifetime
    {
        public CustomWindowsService(IHostEnvironment environment, IHostApplicationLifetime applicationLifetime,
            ILoggerFactory loggerFactory, IOptions<HostOptions> optionsAccessor) : base(environment,
            applicationLifetime, loggerFactory, optionsAccessor)
        {
            Setup();
        }

        public CustomWindowsService(IHostEnvironment environment, IHostApplicationLifetime applicationLifetime,
            ILoggerFactory loggerFactory, IOptions<HostOptions> optionsAccessor,
            IOptions<WindowsServiceLifetimeOptions> windowsServiceOptionsAccessor) : base(environment,
            applicationLifetime, loggerFactory, optionsAccessor, windowsServiceOptionsAccessor)
        {
            Setup();
        }

        private void Setup()
        {
            // Enable windows only events
            if (WindowsServiceHelpers.IsWindowsService())
            {
#pragma warning disable CA1416 // Validate platform compatibility
                CanPauseAndContinue = true;
                CanShutdown = true;
                CanHandleSessionChangeEvent = true;
                CanHandlePowerEvent = true;
#pragma warning restore CA1416 // Validate platform compatibility

            }
        }

        /*
        protected override void OnStart(String[] args)
        {
            base.OnStart(args);
            // Custom start behaviour
        }

        protected override void OnPause()
        {
            base.OnPause();
            // Service continue handler
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            // Service pause handler
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            // System shutdown handler
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
            // Session change handler
        }

        protected override void OnCustomCommand(Int32 command)
        {
            base.OnCustomCommand(command);
            // Custom command handler
        }
        */
    }
}
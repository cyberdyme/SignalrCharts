using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using RealTimeCharts_Server.Models;
using RealTimeCharts_Server.Services;
using RealTimeCharts_Server.Services.BackgroundWorker;
using Serilog;

namespace RealTimeCharts_Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            var environment = "Production";
            if (args.Contains("--environment"))
            {
                var index = Array.IndexOf(args, "--environment");
                if (index < args.Length - 1)
                    // change to read till next space
                    environment = args[index + 1];
            }

            if (isService)
            {
                var mainProcess = Process.GetCurrentProcess().MainModule;
                if (mainProcess != null)
                {
                    var pathToExe = mainProcess.FileName;
                    var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                    if (pathToContentRoot != null) Directory.SetCurrentDirectory(pathToContentRoot);
                }
            }

            // read the application settings from the ApplicationProvider


            var hostBuilder = CreateHostBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseEnvironment(environment)
                .ConfigureHostConfiguration(builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddCommandLine(args);
                })
                .ConfigureServices((context, collection) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .CreateLogger();

                    collection.AddLogging(builder => builder.AddSerilog());
                });

            if (isService)
            {
                await hostBuilder.Build().RunAsync();
                return;
            }

            await hostBuilder.RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    if (WindowsServiceHelpers.IsWindowsService())
                        services.AddSingleton<IHostLifetime, CustomWindowsService>();
                })
                .UseConsoleLifetime()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .CaptureStartupErrors(true).ConfigureAppConfiguration(config =>
                        {
                            config
                                // Used for local settings like connection strings.
                                .AddJsonFile("appsettings.json", true);
                        })
                        .UseSerilog((hostingContext, loggerConfiguration) =>
                        {
                            loggerConfiguration
                                .ReadFrom.Configuration(hostingContext.Configuration)
                                .Enrich.FromLogContext()
                                .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
                                .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);

#if DEBUG
                            // Used to filter out potentially bad data due debugging.
                            // Very useful when doing Seq dashboards and want to remove logs under debugging session.
                            loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif
                        });


                    webBuilder.UseUrls("http://:*:9001", "https://:*:9002");
                    webBuilder.UseIISIntegration();
                    // webBuilder.UseUrls("http://:" + _appSettings.HttpApiPort);
                }).ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IBackgroundWorkerServiceOptions, BackgroundWorkerServiceOptions>();
                    services.AddSingleton<IHostedService, BackgroundWorkerService>();
                    //services.AddHostedService<BackgroundWorkerService>();
                });
    }
}
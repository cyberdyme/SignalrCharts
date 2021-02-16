using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace RealTimeCharts_Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            var environment = "Production";
            if(args.Contains("--environment"))
            {
                var index = Array.IndexOf(args, "--environment");
                if (index < args.Length - 1)
                {
                    // change to read till next space
                    environment = args[index + 1];
                }
            }

            if (isService)
            {
                var mainProcess = Process.GetCurrentProcess().MainModule;
                if (mainProcess != null)
                {
                    var pathToExe = mainProcess.FileName;
                    var pathToContentRoot = Path.GetDirectoryName((pathToExe));
                    if (pathToContentRoot != null)
                    {
                        Directory.SetCurrentDirectory(pathToContentRoot);
                    }
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
                .UseConsoleLifetime()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://:*:9001");
                    webBuilder.UseIISIntegration();
                    // webBuilder.UseUrls("http://:" + _appSettings.HttpApiPort);
                });
    }
}

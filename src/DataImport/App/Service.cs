using System.Threading;
using DasMulli.Win32.ServiceUtils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace RedDeer.DataImport.DataImport.App
{
    public class Service : IWin32Service
    {
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _cts;

        private IWebHost _webHost;
        private bool _stopRequestedByWindows;
        public string ServiceName => Program.ServiceName;

        public Service(ILogger<Service> logger)
        {
            _logger = logger;
            _cts = new CancellationTokenSource();
        }

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            _logger.LogInformation("Service Starting.");
            _webHost = WebHost.CreateDefaultBuilder(startupArguments)
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .UseUrls("http://*:9066/")
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .Build();

            // Make sure the windows service is stopped if the
            // ASP.NET Core stack stops for any reason
            _logger.LogInformation($"Service registering web host");
            _webHost
                .Services
                .GetRequiredService<IApplicationLifetime>()
                .ApplicationStopped
                .Register(() =>
                {
                    if (_stopRequestedByWindows == false)
                    {
                        serviceStoppedCallback();
                    }
                });

            _logger.LogInformation("WebHost Starting.");
            _webHost.Start();
            _logger.LogInformation("WebHost Started.");

            _logger.LogInformation("Service Started.");
        }

        public void Stop()
        {
            _logger.LogInformation("Service Stopping.");

            _stopRequestedByWindows = true;
            _webHost.Dispose();
            _cts.Cancel();

            _logger.LogInformation("Service Stop.");
        }
    }
}
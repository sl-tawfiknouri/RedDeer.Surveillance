namespace RedDeer.Surveillance.App
{
    using System.Threading;

    using DasMulli.Win32.ServiceUtils;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using NLog.Web;

    public class Service : IWin32Service
    {
        private readonly CancellationTokenSource _cts;

        private readonly ILogger _logger;

        private bool _stopRequestedByWindows;

        private IHost _host;
        private StructureMapServiceProviderFactory _structureMapServiceProviderFactory;

        public Service(ILogger<Service> logger)
        {
            this._logger = logger;
            this._cts = new CancellationTokenSource();
        }

        public string ServiceName => Program.ServiceName;

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            this._logger.LogInformation("Service Starting.");

            this._structureMapServiceProviderFactory = new StructureMapServiceProviderFactory(StructureMapContainer.Instance);
            this._host = Host.CreateDefaultBuilder(startupArguments)
                .UseServiceProviderFactory(this._structureMapServiceProviderFactory)
                .ConfigureWebHostDefaults(webBuilder => 
                { 
                    webBuilder
                        .UseStartup<Startup>()
                        .UseUrls("http://*:9065/"); 
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .Build();

            // Make sure the windows service is stopped if the
            // ASP.NET Core stack stops for any reason
            this._host
                .Services
                .GetRequiredService<IHostApplicationLifetime>()
                .ApplicationStopped
                .Register(() =>
                {
                    if (this._stopRequestedByWindows == false) 
                        serviceStoppedCallback();
                });

            this._logger.LogInformation("WebHost Starting.");
            this._host.Start();
            this._logger.LogInformation("WebHost Started.");

            this._logger.LogInformation("Service Started.");
        }

        public void Stop()
        {
            this._logger.LogInformation("Service Stopping.");

            this._stopRequestedByWindows = true;
            this._host.Dispose();
            this._structureMapServiceProviderFactory?.Dispose();
            this._cts.Cancel();

            this._logger.LogInformation("Service Stop.");
        }
    }
}
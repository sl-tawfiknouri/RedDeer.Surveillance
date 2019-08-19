namespace RedDeer.Surveillance.App
{
    using System.Threading;

    using DasMulli.Win32.ServiceUtils;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using NLog.Web;

    public class Service : IWin32Service
    {
        private readonly CancellationTokenSource _cts;

        private readonly ILogger _logger;

        private bool _stopRequestedByWindows;

        private IWebHost _webHost;

        public Service(ILogger<Service> logger)
        {
            this._logger = logger;
            this._cts = new CancellationTokenSource();
        }

        public string ServiceName => Program.ServiceName;

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            this._logger.LogInformation("Service Starting.");
            this._webHost = WebHost.CreateDefaultBuilder(startupArguments).UseStartup<Startup>()
                .UseDefaultServiceProvider(options => options.ValidateScopes = false).UseUrls("http://*:9065/")
                .ConfigureLogging(
                    logging =>
                        {
                            logging.ClearProviders();
                            logging.SetMinimumLevel(LogLevel.Trace);
                        }).UseNLog().Build();

            // Make sure the windows service is stopped if the
            // ASP.NET Core stack stops for any reason
            this._webHost.Services.GetRequiredService<IApplicationLifetime>().ApplicationStopped.Register(
                () =>
                    {
                        if (this._stopRequestedByWindows == false) serviceStoppedCallback();
                    });

            this._logger.LogInformation("WebHost Starting.");
            this._webHost.Start();
            this._logger.LogInformation("WebHost Started.");

            this._logger.LogInformation("Service Started.");
        }

        public void Stop()
        {
            this._logger.LogInformation("Service Stopping.");

            this._stopRequestedByWindows = true;
            this._webHost.Dispose();
            this._cts.Cancel();

            this._logger.LogInformation("Service Stop.");
        }
    }
}
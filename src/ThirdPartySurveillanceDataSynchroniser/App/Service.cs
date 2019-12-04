using DasMulli.Win32.ServiceUtils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace DataSynchroniser.App
{
    public class Service : IWin32Service
    {
        private ILogger<Service> _logger;
        private StructureMapServiceProviderFactory _structureMapServiceProviderFactory;
        private IHost _host;

        public Service(ILogger<Service> logger)
        {
            this._logger = logger;
        }

        public string ServiceName => Program.ServiceName;

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            this._logger.LogInformation("Service Starting.");

            _structureMapServiceProviderFactory = new StructureMapServiceProviderFactory();
            this._host = Host.CreateDefaultBuilder(startupArguments)
                .UseServiceProviderFactory(_structureMapServiceProviderFactory)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .Build();
            
            this._logger.LogInformation("Host Starting.");
            this._host.Start();
            this._logger.LogInformation("Host Started.");
            
            this._logger.LogInformation("Service Started.");
        }

        public void Stop()
        {
            this._logger.LogInformation("Service Stopping.");

            this._host?.Dispose();
            this._structureMapServiceProviderFactory?.Dispose();

            this._logger.LogInformation("Service Stop.");
        }
    }
}
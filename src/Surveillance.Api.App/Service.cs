using System.Collections.Generic;
using System.Net;
using System.Threading;
using Amazon.DynamoDBv2;
using DasMulli.Win32.ServiceUtils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Surveillance.Api.App.Configuration;
using Microsoft.Extensions.Configuration;

namespace Surveillance.Api.App
{
    public class Service : IWin32Service
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CancellationTokenSource _cts;

        private IWebHost _webHost;
        private bool _stopRequestedByWindows;
        public string ServiceName => Program.ServiceName;

        public Service(ILogger<Service> logger)
        {
            _cts = new CancellationTokenSource();
        }

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            Logger.Log(NLog.LogLevel.Info, "Service Starting.");

            _webHost = CreateWebHostBuilder(startupArguments).Build();

            // Make sure the windows service is stopped if the
            // ASP.NET Core stack stops for any reason
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

            Logger.Log(NLog.LogLevel.Info, "WebHost Starting.");
            _webHost.Start();
            Logger.Log(NLog.LogLevel.Info, "WebHost Started.");

            Logger.Log(NLog.LogLevel.Info, "Service Started.");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(i => i.AddInMemoryCollection(GetDynamoDbConfig()))
            .UseStartup<Startup>()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            })
            .UseNLog();

        private static IEnumerable<KeyValuePair<string, string>> GetDynamoDbConfig()
        {
            var client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
                ProxyCredentials = CredentialCache.DefaultCredentials
            });
            var environmentService = new EnvironmentService();
            var logger = LogManager.GetLogger(nameof(DynamoDbConfigurationProvider));

            var config = new DynamoDbConfigurationProvider(environmentService, client, logger);

            return config.Build();
        }

        public void Stop()
        {
            Logger.Log(NLog.LogLevel.Info, "Service Stopping.");

            _stopRequestedByWindows = true;
            _webHost.Dispose();
            _cts.Cancel();

            Logger.Log(NLog.LogLevel.Info, "Service Stop.");
        }
    }
}
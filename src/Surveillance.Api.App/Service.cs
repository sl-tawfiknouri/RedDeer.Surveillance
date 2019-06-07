using System.Threading;
using DasMulli.Win32.ServiceUtils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Microsoft.Extensions.Configuration;
using Dazinator.AspNet.Extensions.FileProviders;
using Surveillance.Api.App.Configuration;

namespace Surveillance.Api.App
{
    public class Service : IWin32Service
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CancellationTokenSource _cts;

        private IWebHost _webHost;
        private bool _stopRequestedByWindows;
        public string ServiceName => Program.ServiceName;

        public IStartupConfig StartupConfig { get; set; } = new StartupConfig();

        public Service(ILogger<Service> logger)
        {
            _cts = new CancellationTokenSource();
        }

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            Logger.Log(NLog.LogLevel.Info, "Service Starting.");

            string url = null;
            string dynamoDbConfigJson = null;

            if (!StartupConfig.IsTest)
            {
                // No need to spend time checking dynamo db if in test mode
                dynamoDbConfigJson = DynamoDbConfigurationProviderFactory.Create().GetJson(); 
                
                if (!string.IsNullOrWhiteSpace(dynamoDbConfigJson))
                {
                    var builder = new ConfigurationBuilder();

                    var provider = new InMemoryFileProvider();
                    provider.Directory.AddFile("/", new StringFileInfo(dynamoDbConfigJson, "appsetting.dynamodb.json"));
                    provider.EnsureFile("/appsetting.dynamodb.json");

                    builder.AddJsonFile(provider, "/appsetting.dynamodb.json", false, false);

                    var config = builder.Build();

                    url = config.GetValue<string>("SurveillanceApiUrl");
                }
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                url = "https://localhost:8888";
            }
            
            _webHost = CreateWebHostBuilder(startupArguments, dynamoDbConfigJson, url, StartupConfig).Build();

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

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, string json, string url, IStartupConfig startupConfig) =>
        WebHost
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(i => 
            {
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var provider = new InMemoryFileProvider();
                    provider.Directory.AddFile("/", new StringFileInfo(json, "appsettings.dynamodb.json"));

                    i.AddJsonFile(provider, "appsettings.dynamodb.json", false, true);
                }
            })
            .ConfigureServices(services => services.AddScoped(x => startupConfig))
            .UseStartup<Startup>()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            })
            .UseKestrel()
            .UseUrls(url)
            .UseNLog();

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
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

            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(startupArguments);

            if (!StartupConfig.IsTest)
            {
                // No need to spend time checking dynamo db if in test mode
                dynamoDbConfigJson = DynamoDbConfigurationProviderFactory.Create().GetJson(); 
                
                if (!string.IsNullOrWhiteSpace(dynamoDbConfigJson))
                {
                    var dynamoAppSettingsFileName = "appsettings.dynamodb.json";
                    var provider = new InMemoryFileProvider();
                    provider.Directory.AddFile("/", new StringFileInfo(dynamoDbConfigJson, dynamoAppSettingsFileName));
                    provider.EnsureFile($"/{dynamoAppSettingsFileName}");

                    builder.AddJsonFile(provider, $"/{dynamoAppSettingsFileName}", false, false);
                }
            }

            var config = builder.Build();
            url = config.GetValue<string>("SurveillanceApiUrl");

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
            .ConfigureAppConfiguration(configurationBuilder => ConfigureAppConfiguration(configurationBuilder, json))
            .ConfigureServices(services => services.AddScoped(x => startupConfig))
            .UseStartup<Startup>()
            .ConfigureLogging(ConfigureLogging)
            .UseKestrel()
            .UseUrls(url)
            .UseNLog();

        private static void ConfigureAppConfiguration(IConfigurationBuilder configurationBuilder, string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            var provider = new InMemoryFileProvider();
            provider.Directory.AddFile("/", new StringFileInfo(json, "appsettings.dynamodb.json"));

            configurationBuilder.AddJsonFile(provider, "appsettings.dynamodb.json", false, true);
        }

        private static void ConfigureLogging(WebHostBuilderContext context, ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

            if (context.HostingEnvironment.IsDevelopment())
            {
                logging.AddConsole();
                // logging.AddFilter<Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider>(Microsoft.EntityFrameworkCore.DbLoggerCategory.Database.Command.Name, Microsoft.Extensions.Logging.LogLevel.Information);
            }
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
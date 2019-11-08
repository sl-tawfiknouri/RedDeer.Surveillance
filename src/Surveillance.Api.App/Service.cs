namespace Surveillance.Api.App
{
    using System.Threading;

    using DasMulli.Win32.ServiceUtils;

    using Dazinator.AspNet.Extensions.FileProviders;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Hosting;

    using NLog;
    using NLog.Web;

    using Surveillance.Api.App.Configuration;

    using LogLevel = NLog.LogLevel;

    public class Service : IWin32Service
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource _cts;

        private bool _stopRequestedByWindows;

        private IWebHost _webHost;

        public Service(ILogger<Service> logger)
        {
            this._cts = new CancellationTokenSource();
        }

        public string ServiceName => Program.ServiceName;

        public IStartupConfig StartupConfig { get; set; } = new StartupConfig();

        public static IWebHostBuilder CreateWebHostBuilder(
            string[] args,
            string json,
            string url,
            IStartupConfig startupConfig)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    configurationBuilder => ConfigureAppConfiguration(configurationBuilder, json))
                .ConfigureServices(services => services.AddScoped(x => startupConfig)).UseStartup<Startup>()
                .ConfigureLogging(ConfigureLogging).UseKestrel().UseUrls(url).UseNLog();
        }

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            Logger.Log(LogLevel.Info, "Service Starting.");

            string url = null;
            string dynamoDbConfigJson = null;

            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(startupArguments);

            if (!this.StartupConfig.IsTest)
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

            if (string.IsNullOrWhiteSpace(url)) url = "https://localhost:8888";

            this._webHost = CreateWebHostBuilder(startupArguments, dynamoDbConfigJson, url, this.StartupConfig).Build();

            // Make sure the windows service is stopped if the
            // ASP.NET Core stack stops for any reason
            this._webHost.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopped.Register(
                () =>
                    {
                        if (this._stopRequestedByWindows == false) serviceStoppedCallback();
                    });

            Logger.Log(LogLevel.Info, "WebHost Starting.");
            this._webHost.Start();
            Logger.Log(LogLevel.Info, "WebHost Started.");

            Logger.Log(LogLevel.Info, "Service Started.");
        }

        public void Stop()
        {
            Logger.Log(LogLevel.Info, "Service Stopping.");

            this._stopRequestedByWindows = true;
            this._webHost.Dispose();
            this._cts.Cancel();

            Logger.Log(LogLevel.Info, "Service Stop.");
        }

        private static void ConfigureAppConfiguration(IConfigurationBuilder configurationBuilder, string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;

            var provider = new InMemoryFileProvider();
            provider.Directory.AddFile("/", new StringFileInfo(json, "appsettings.dynamodb.json"));

            configurationBuilder.AddJsonFile(provider, "appsettings.dynamodb.json", false, true);
        }

        private static void ConfigureLogging(WebHostBuilderContext context, ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

            if (context.HostingEnvironment.IsDevelopment()) logging.AddConsole();
        }
    }
}
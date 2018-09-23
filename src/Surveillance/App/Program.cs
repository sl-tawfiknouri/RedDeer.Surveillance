using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DasMulli.Win32.ServiceUtils;
using Microsoft.Extensions.Configuration;
using NLog;
using StructureMap;
using Surveillance;
using Surveillance.Configuration;
using Surveillance.Configuration.Interfaces;
using Surveillance.DataLayer;
using Surveillance.DataLayer.Configuration;
using Surveillance.DataLayer.Configuration.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace RedDeer.Surveillance.App
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string RunAsServiceFlag = "--run-as-service";
        private const string RunAsSystemdServiceFlag = "--systemd-service";
        private const string RegisterServiceFlag = "--register-service";
        private const string UnregisterServiceFlag = "--unregister-service";

        internal const string ServiceName = "RedDeer.SurveillanceService";
        private const string ServiceDisplayName = "RedDeer Surveillance Service";
        private const string ServiceDescription = "RedDeer Surveillance Service";

        private static Container Container { get; set; }

        public static void Main(string[] args)
        {
            try
            {
                Container = new Container();

                var configurationBuilder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();

                var dbConfiguration = BuildDatabaseConfiguration(configurationBuilder);
                Container.Inject(typeof(INetworkConfiguration), BuildNetworkConfiguration(configurationBuilder));
                Container.Inject(typeof(IElasticSearchConfiguration), dbConfiguration);
                Container.Inject(typeof(IAwsConfiguration), dbConfiguration);
                Container.Inject(typeof(IRuleConfiguration), BuildRuleConfiguration(configurationBuilder));

                Container.Configure(config =>
                {
                    config.IncludeRegistry<DataLayerRegistry>();
                    config.IncludeRegistry<SurveillanceRegistry>();
                    config.IncludeRegistry<AppRegistry>();
                });

                var startUpTaskRunner = Container.GetInstance<IStartUpTaskRunner>();
                startUpTaskRunner.Run().Wait();

                ProcessArguments(args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine($"An error ocurred: {ex.Message}");
            }
        }

        private static INetworkConfiguration BuildNetworkConfiguration(IConfigurationRoot configurationBuilder)
        {
           var networkConfiguration = new NetworkConfiguration
            {
                SurveillanceServiceEquityDomain = configurationBuilder.GetValue<string>("SurveillanceServiceEquityDomain"),
                SurveillanceServiceEquityPort = configurationBuilder.GetValue<string>("SurveillanceServiceEquityPort"),

                SurveillanceServiceTradeDomain = configurationBuilder.GetValue<string>("SurveillanceServiceTradeDomain"),
                SurveillanceServiceTradePort = configurationBuilder.GetValue<string>("SurveillanceServiceTradePort"),
            };

            return networkConfiguration;
        }

        private static IElasticSearchConfiguration BuildDatabaseConfiguration(IConfigurationRoot configurationBuilder)
        {
            var networkConfiguration = new ElasticSearchConfiguration
            {
                IsEc2Instance = configurationBuilder.GetValue<bool?>("IsEc2Instance") ?? false,
                AwsSecretKey = configurationBuilder.GetValue<string>("AwsSecretKey"),
                AwsAccessKey = configurationBuilder.GetValue<string>("AwsAccessKey"),
                ScheduledRuleQueueName = configurationBuilder.GetValue<string>("ScheduledRuleQueueName"),
                CaseMessageQueueName = configurationBuilder.GetValue<string>("CaseMessageQueueName"),
                ElasticSearchProtocol = configurationBuilder.GetValue<string>("ElasticSearchProtocol"),
                ElasticSearchDomain = configurationBuilder.GetValue<string>("ElasticSearchDomain"),
                ElasticSearchPort = configurationBuilder.GetValue<string>("ElasticSearchPort")
            };

            return networkConfiguration;
        }

        private static IRuleConfiguration BuildRuleConfiguration(IConfigurationRoot configurationBuilder)
        {
            var ruleConfiguration = new RuleConfiguration
            {
                CancelledOrderDeduplicationDelaySeconds = configurationBuilder.GetValue<int?>("CancelledOrderDeduplicationDelaySeconds")
            };

            return ruleConfiguration;
        }

        private static void ProcessArguments(string[] args)
        {
            if (args.Contains(RunAsServiceFlag))
            {
                Logger.Info($"Run As Service Flag Found ({RunAsServiceFlag}).");
                RunAsService(args);
            }
            else if (args.Contains(RunAsSystemdServiceFlag))
            {
                Logger.Info($"Run As Systemd Service Flag Found ({RunAsSystemdServiceFlag}).");
                RunAsSystemdService(args);
            }
            else if (args.Contains(RegisterServiceFlag))
            {
                Logger.Info($"Register Service Flag Found ({RegisterServiceFlag}).");
                RegisterService();
            }
            else if (args.Contains(UnregisterServiceFlag))
            {
                Logger.Info($"Unregister Service Flag Found ({UnregisterServiceFlag}).");
                UnregisterService();
            }           
            else
            {
                Logger.Info($"No Flags Found.");
                RunInteractive(args);
            }
        }

        private static void RunAsService(string[] args)
        {
            var service = Container.GetInstance<Service>();
            var serviceHost = new Win32ServiceHost(service);
            serviceHost.Run();
        }

        private static void RunAsSystemdService(string[] args)
        {
            // Register sigterm event handler. 
            var sigterm = new ManualResetEventSlim();
            AssemblyLoadContext.Default.Unloading += x => {
                Logger.Info($"Sigterm triggered.");
                sigterm.Set();
            };

            var service = Container.GetInstance<Service>();
            service.Start(new string[0], () => { });
            sigterm.Wait();
            service.Stop();
        }

        private static void RunInteractive(string[] args)
        {
            var service = Container.GetInstance<Service>();
            service.Start(new string[0], () => { });
            Console.WriteLine("Running interactively, press enter to stop.");
            Console.ReadLine();
            service.Stop();
        }

        private static void RunOnConsole(Func<Container, CancellationToken, Task> asyncAction)
        {
            var cts = new CancellationTokenSource();
            Logger.Info("Waiting on task...");
            var task = Task.Run(async () =>
            {
                await asyncAction(Container, cts.Token);
                Logger.Info("End of task");
            }, cts.Token);
            // uncomment to allow for cancelling task early
            /* Console.ReadLine(); 
            cts.Cancel(); /* */
            Logger.Info("Waiting for end of task...");
            task.Wait(cts.Token);
            Logger.Info("End of application");
        }

        private static void RegisterService()
        {
            // Environment.GetCommandLineArgs() includes the current DLL from a "dotnet my.dll --register-service" call, which is not passed to Main()
            var remainingArgs = Environment.GetCommandLineArgs()
                .Where(arg => arg != RegisterServiceFlag)
                .Select(EscapeCommandLineArgument)
                .Append(RunAsServiceFlag);

            var host = Process.GetCurrentProcess().MainModule.FileName;

            if (!host.EndsWith("dotnet.exe", StringComparison.OrdinalIgnoreCase))
            {
                // For self-contained apps, skip the dll path
                remainingArgs = remainingArgs.Skip(1);
            }

            var fullServiceCommand = host + " " + string.Join(" ", remainingArgs);

            // Do not use LocalSystem in production.. but this is good for demos as LocalSystem will have access to some random git-clone path
            // Note that when the service is already registered and running, it will be reconfigured but not restarted
            var serviceDefinition = new ServiceDefinitionBuilder(ServiceName)
                .WithDisplayName(ServiceDisplayName)
                .WithDescription(ServiceDescription)
                .WithBinaryPath(fullServiceCommand)
                .WithCredentials(Win32ServiceCredentials.LocalSystem)
                .WithAutoStart(true)
                .Build();

            Console.WriteLine(ServiceName);
            Console.WriteLine(ServiceDisplayName);
            Console.WriteLine(ServiceDescription);
            Console.WriteLine(fullServiceCommand);

            new Win32ServiceManager().CreateOrUpdateService(serviceDefinition, startImmediately: true);

            Console.WriteLine($@"Successfully registered and started service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
        }

        private static void UnregisterService()
        {
            new Win32ServiceManager()
                .DeleteService(ServiceName);

            Console.WriteLine($@"Successfully unregistered service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
        }

        private static string EscapeCommandLineArgument(string arg)
        {
            // http://stackoverflow.com/a/6040946/784387
            arg = Regex.Replace(arg, @"(\\*)" + "\"", @"$1$1\" + "\"");
            arg = "\"" + Regex.Replace(arg, @"(\\+)$", @"$1$1") + "\"";
            return arg;
        }
    }
}

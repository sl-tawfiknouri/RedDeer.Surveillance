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
using Relay.Configuration;
using StructureMap;

namespace RedDeer.Relay.App
{
    public class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private const string RunAsServiceFlag = "--run-as-service";
        private const string RunAsSystemdServiceFlag = "--systemd-service";
        private const string RegisterServiceFlag = "--register-service";
        private const string UnregisterServiceFlag = "--unregister-service";

        internal const string ServiceName = "RedDeer.RelayService";
        private const string ServiceDisplayName = "RedDeer Relay Service";
        private const string ServiceDescription = "RedDeer Relay Service";

        private static Container _container { get; set; }

        public static void Main(string[] args)
        {
            try
            {
                _container = new Container();

                _container.Inject(typeof(INetworkConfiguration), BuildConfiguration());

                _container.Configure(config =>
                {
                    config.IncludeRegistry<RelayRegistry>();
                    config.IncludeRegistry<AppRegistry>();
                });

                var startUpTaskRunner = _container.GetInstance<IStartUpTaskRunner>();
                startUpTaskRunner.Run().Wait();

                ProcessArguments(args);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                Console.WriteLine($"An error ocurred: {ex.Message}");
            }
        }

        private static Configuration BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var networkConfiguration = new Configuration
            {
                RelayServiceEquityDomain = configurationBuilder.GetValue<string>("RelayServiceEquityDomain"),
                RelayServiceEquityPort = configurationBuilder.GetValue<string>("RelayServiceEquityPort"),
                SurveillanceServiceEquityDomain = configurationBuilder.GetValue<string>("SurveillanceServiceEquityDomain"),
                SurveillanceServiceEquityPort = configurationBuilder.GetValue<string>("SurveillanceServiceEquityPort"),
            };

            return networkConfiguration;
        }

        private static void ProcessArguments(string[] args)
        {
            if (args.Contains(RunAsServiceFlag))
            {
                _logger.Info($"Run As Service Flag Found ({RunAsServiceFlag}).");
                RunAsService(args);
            }
            else if (args.Contains(RunAsSystemdServiceFlag))
            {
                _logger.Info($"Run As Systemd Service Flag Found ({RunAsSystemdServiceFlag}).");
                RunAsSystemdService(args);
            }
            else if (args.Contains(RegisterServiceFlag))
            {
                _logger.Info($"Register Service Flag Found ({RegisterServiceFlag}).");
                RegisterService();
            }
            else if (args.Contains(UnregisterServiceFlag))
            {
                _logger.Info($"Unregister Service Flag Found ({UnregisterServiceFlag}).");
                UnregisterService();
            }           
            else
            {
                _logger.Info($"No Flags Found.");
                RunInteractive(args);
            }
        }

        private static void RunAsService(string[] args)
        {
            var service = _container.GetInstance<Service>();
            var serviceHost = new Win32ServiceHost(service);
            serviceHost.Run();
        }

        private static void RunAsSystemdService(string[] args)
        {
            // Register sigterm event handler. 
            var sigterm = new ManualResetEventSlim();
            AssemblyLoadContext.Default.Unloading += x => {
                _logger.Info($"Sigterm triggered.");
                sigterm.Set();
            };

            var service = _container.GetInstance<Service>();
            service.Start(new string[0], () => { });
            sigterm.Wait();
            service.Stop();
        }

        private static void RunInteractive(string[] args)
        {
            var service = _container.GetInstance<Service>();
            service.Start(new string[0], () => { });
            Console.WriteLine("Running interactively, press enter to stop.");
            Console.ReadLine();
            service.Stop();
        }

        private static void RunOnConsole(Func<Container, CancellationToken, Task> asyncAction)
        {
            var cts = new CancellationTokenSource();
            _logger.Info("Waiting on task...");
            var task = Task.Run(async () =>
            {
                await asyncAction(_container, cts.Token);
                _logger.Info("End of task");
            });
            // uncomment to allow for cancelling task early
            /* Console.ReadLine(); 
            cts.Cancel(); /* */
            _logger.Info("Waiting for end of task...");
            task.Wait();
            _logger.Info("End of application");
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

﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using System.Threading;
using DasMulli.Win32.ServiceUtils;
using DataImport;
using DataImport.Configuration;
using DataImport.Configuration.Interfaces;
using Microsoft.Extensions.Configuration;
using NLog;
using StructureMap;
using Surveillance.DataLayer;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.System.Auditing;
using Surveillance.System.Auditing.Context;
using Surveillance.System.DataLayer;
using Surveillance.System.DataLayer.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Utilities.Aws_IO.Interfaces;

// ReSharper disable UnusedParameter.Local

namespace RedDeer.DataImport.DataImport.App
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string RunAsServiceFlag = "--run-as-service";
        private const string RunAsSystemServiceFlag = "--systemd-service";
        private const string RegisterServiceFlag = "--register-service";
        private const string UnRegisterServiceFlag = "--unregister-service";

        internal const string ServiceName = "RedDeer.RelayService";
        private const string ServiceDisplayName = "RedDeer Relay Service";
        private const string ServiceDescription = "RedDeer Relay Service";

        private static Container Container { get; set; }

        public static void Main(string[] args)
        {
            try
            {
                Container = new Container();

                var builtConfig = BuildConfiguration();
                Container.Inject(typeof(INetworkConfiguration), builtConfig);
                Container.Inject(typeof(IUploadConfiguration), builtConfig);
                Container.Inject(typeof(ISystemDataLayerConfig), builtConfig);
                SystemProcessContext.ProcessType = SystemProcessType.RelayService;

                var builtDataLayerConfig = BuildDataLayerConfiguration();
                Container.Inject(typeof(IAwsConfiguration), builtDataLayerConfig);
                Container.Inject(typeof(IDataLayerConfiguration), builtDataLayerConfig);

                Container.Configure(config =>
                {
                    config.IncludeRegistry<RelayRegistry>();
                    config.IncludeRegistry<AppRegistry>();
                    config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                    config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                    config.IncludeRegistry<DataLayerRegistry>();
                });

                Container.Inject(typeof(ISystemDataLayerConfig), builtConfig);

                var startUpTaskRunner = Container.GetInstance<IStartUpTaskRunner>();
                startUpTaskRunner.Run().Wait();

                ProcessArguments(args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static Configuration BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();

            return builder.Build(configurationBuilder);
        }

        private static IDataLayerConfiguration BuildDataLayerConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();

            return builder.BuildData(configurationBuilder);
        }

        private static void ProcessArguments(string[] args)
        {
            if (args.Contains(RunAsServiceFlag))
            {
                Logger.Info($"Run As Service Flag Found ({RunAsServiceFlag}).");
                RunAsService(args);
            }
            else if (args.Contains(RunAsSystemServiceFlag))
            {
                Logger.Info($"Run As Systemd Service Flag Found ({RunAsSystemServiceFlag}).");
                RunAsSystemService(args);
            }
            else if (args.Contains(RegisterServiceFlag))
            {
                Logger.Info($"Register Service Flag Found ({RegisterServiceFlag}).");
                RegisterService();
            }
            else if (args.Contains(UnRegisterServiceFlag))
            {
                Logger.Info($"Unregister Service Flag Found ({UnRegisterServiceFlag}).");
                UnRegisterService();
            }           
            else
            {
                Logger.Info("No Flags Found.");
                RunInteractive(args);
            }
        }

        private static void RunAsService(string[] args)
        {
            var service = Container.GetInstance<Service>();
            var serviceHost = new Win32ServiceHost(service);
            serviceHost.Run();
        }

        private static void RunAsSystemService(string[] args)
        {
            // Register sigterm event handler. 
            var sigterm = new ManualResetEventSlim();
            AssemblyLoadContext.Default.Unloading += x => {
                Logger.Info("Sigterm triggered.");
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

        private static void UnRegisterService()
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

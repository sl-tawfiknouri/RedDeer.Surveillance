﻿using Microsoft.Extensions.DependencyInjection;
using NLog;
using Surveillance.Api.App;
using Surveillance.Api.Client;
using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests
{
    class Program
    {
        public static Logger Logger => LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            // TestOverrides
            var startupConfig = new StartupConfig
            {
                IsTest = true,
                ConfigureTestServices = (services) =>
                {
                    services.AddScoped<IGraphQlDbContextFactory, DbContextFactory>();
                }
            };

            // NLog
            var config = new NLog.Config.LoggingConfiguration();
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            LogManager.Configuration = config; // set config when nlog used from LogManager.GetCurrentClassLogger()
            NLog.Web.NLogBuilder.ConfigureNLog(config); // set config when nlog used from asp net core

            // backend api service
            var service = new Service(null);
            service.StartupConfig = startupConfig;
            service.Start(new string[0], () => { });

            // attempt request
            var client = new ApiClient(new HttpClientHandler
            {
                UseProxy = false,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            });
            try
            {
                var count = client.RuleBreachesCountAsync().Result;
                Logger.Info($"Rules Breaches Count = {count}");
            }
            catch (Exception e)
            {
                Logger.Info("Exception in graphql request");
                Logger.Error(e);
            }

            // wait for user interaction
            Logger.Info("Waiting for user keyboard...");
            Console.ReadLine();

            // cleanup
            service.Stop();
            Logger.Info("End of application");
        }
    }
}

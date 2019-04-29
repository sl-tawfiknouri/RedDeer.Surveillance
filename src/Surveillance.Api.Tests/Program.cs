using Microsoft.Extensions.DependencyInjection;
using NLog;
using Surveillance.Api.App;
using Surveillance.Api.Client;
using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
using System;
using System.Collections.Generic;
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
            var testOverrides = new TestOverrides
            {
                Config = new Dictionary<string, string>
                {
                    ["Secret-Key-Jwt"] = @"fTjWnZr4u7x!A%D*G-KaPdSgUkXp2s5v8y/B?E(H+MbQeThWmYq3t6w9z$C&F)J@NcRfUjXn2r4u7x!A%D*G-KaPdSgVkYp3s6v8y/B?E(H+MbQeThWmZq4t7w!z$C&F"
                },
                ConfigureServices = (services) =>
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
            service.TestOverrides = testOverrides;
            service.Start(new string[0], () => { });

            // attempt request
            var client = new ApiClient();
            try
            {
                client.RequestAsync().Wait();
            }
            catch (Exception e)
            {
                Logger.Info(e);
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

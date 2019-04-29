using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NUnit.Framework;
using Surveillance.Api.App;
using Surveillance.Api.Client;
using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
using Surveillance.Api.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Surveillance.Api.Tests.Tests
{
    [SetUpFixture]
    public class SetUp
    {
        private Service _service;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            // NLog
            var config = new NLog.Config.LoggingConfiguration();
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            LogManager.Configuration = config; // set config when nlog used from LogManager.GetCurrentClassLogger()
            NLog.Web.NLogBuilder.ConfigureNLog(config); // set config when nlog used from asp net core

            // Database
            var optionBuilders = new DbContextOptionsBuilder<Infrastructure.DbContext>();
            optionBuilders.UseInMemoryDatabase("inMemoryDB", (inMemoryDbContextOptionsBuilder) => { });
            var context = new Infrastructure.DbContext(optionBuilders.Options);
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            Dependencies.DbContext = context;

            // Backend api StartupConfig
            var startupConfig = new StartupConfig
            {
                IsTest = true,
                ConfigureTestServices = (services) =>
                {
                    services.AddScoped<IGraphQlDbContextFactory, DbContextFactory>();
                }
            };

            // Backend api service
            var service = new Service(null);
            service.StartupConfig = startupConfig;
            service.Start(new string[0], () => { });
            _service = service;

            // ApiClient
            var client = new ApiClient(new HttpClientHandler
            {
                UseProxy = false,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            });
            Dependencies.ApiClient = client;
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            _service.Stop();
        }

    }
}

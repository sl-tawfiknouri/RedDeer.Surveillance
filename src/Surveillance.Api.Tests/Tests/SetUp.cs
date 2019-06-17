using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NUnit.Framework;
using RedDeer.Security.Core;
using RedDeer.Security.Core.Abstractions;
using RedDeer.Security.Core.Services;
using Surveillance.Api.App;
using RedDeer.Surveillance.Api.Client;
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
            var factory = new DbContextFactory();
            Dependencies.DbContext = factory.Build() as Infrastructure.DbContext;

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

            var basePathUrl = "https://localhost:18888/";
            var commandLineArgs = new string[] { $"SurveillanceApiUrl={basePathUrl}" };

            var service = new Service(null)
            {
                StartupConfig = startupConfig
            };

            service.Start(commandLineArgs, () => { });
            _service = service;

            // ApiClient
            var jwtToken = new JwtToken(
                expires: DateTime.UtcNow.AddHours(1),
                issuer: new ComponentName("dev", "test", ScopeType.ClientService),
                audience: new ComponentName("dev", "test", ScopeType.ClientService),
                claims: new List<IClaim>
                {
                    new Claim(ScopeType.Surveillance, AccessType.Read)
                });
            var secretKey = "fTjWnZr4u7x!A%D*G-KaPdSgUkXp2s5v8y/B?E(H+MbQeThWmYq3t6w9z$C&F)J@NcRfUjXn2r4u7x!A%D*G-KaPdSgVkYp3s6v8y/B?E(H+MbQeThWmZq4t7w!z$C&F";
            var jwtTokenService = new JwtTokenService();
            var bearer = jwtTokenService.Generate(jwtToken, secretKey);

            var url = new UriBuilder(basePathUrl) { Path = "graphql/surveillance" }.ToString();
            var client = new ApiClient(url, bearer, new HttpClientHandler
            {
                UseProxy = false
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

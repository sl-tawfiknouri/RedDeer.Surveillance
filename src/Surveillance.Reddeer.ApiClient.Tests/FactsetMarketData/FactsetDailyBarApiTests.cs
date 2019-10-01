namespace Surveillance.Reddeer.ApiClient.Tests.FactsetMarketData
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Infrastructure.Network.HttpClient.Interfaces;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using PollyFacade.Policies.Interfaces;

    using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.FactsetMarketData;
    using Surveillance.Reddeer.ApiClient.Tests.Helpers;

    /// <summary>
    /// The daily bar repository tests.
    /// </summary>
    [TestFixture]
    public class FactsetDailyBarApiTests
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        private IApiClientConfiguration configuration;

        /// <summary>
        /// The http client factory.
        /// </summary>
        private IHttpClientFactory httpClientFactory;

        /// <summary>
        /// The policy factory.
        /// </summary>
        private IPolicyFactory policyFactory;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<FactsetDailyBarApi> logger;

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.policyFactory = A.Fake<IPolicyFactory>();
            this.httpClientFactory = A.Fake<IHttpClientFactory>();
            this.configuration = TestHelpers.Config();
            this.logger = A.Fake<ILogger<FactsetDailyBarApi>>();
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        [Explicit]
        public async Task Get()
        {
            var repo = new FactsetDailyBarApi(
                this.configuration,
                this.httpClientFactory,
                this.policyFactory,
                this.logger);

            var message = new FactsetSecurityDailyRequest
              {
                  Requests = new List<FactsetSecurityRequestItem>
                                 {
                                     new FactsetSecurityRequestItem
                                         {
                                             Figi = "BBG000C6K6G9",
                                             From = new DateTime(2018, 01, 01),
                                             To = new DateTime(2018, 01, 05)
                                         }
                                 }
              };

            await repo.GetWithTransientFaultHandlingAsync(message);

            Assert.IsTrue(true);
        }

        /// <summary>
        /// The heart beat.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        [Explicit]
        public async Task Heartbeating()
        {
            var repo = new FactsetDailyBarApi(
                this.configuration,
                this.httpClientFactory,
                this.policyFactory,
                this.logger);
            var cts = new CancellationTokenSource();

            await repo.HeartBeatingAsync(cts.Token);

            Assert.IsTrue(true);
        }
    }
}
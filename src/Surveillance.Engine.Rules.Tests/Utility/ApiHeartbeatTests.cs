namespace Surveillance.Engine.Rules.Tests.Utility
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.Utility;
    using Surveillance.Reddeer.ApiClient.Enrichment.Interfaces;
    using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;
    using Surveillance.Reddeer.ApiClient.RuleParameter.Interfaces;

    [TestFixture]
    public class ApiHeartbeatTests
    {
        private IEnrichmentApi _enrichmentApi;

        private IExchangeRateApiCachingDecorator _exchangeRateApi;

        private ILogger<ApiHeartbeat> _logger;

        private IMarketOpenCloseApiCachingDecorator _marketApi;

        private IRuleParameterApi _ruleApi;

        [Test]
        public void Constructor_NullExchangeRateApi_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new ApiHeartbeat(null, this._marketApi, this._ruleApi, this._enrichmentApi, this._logger));
        }

        [Test]
        public void Constructor_NullMarketApi_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new ApiHeartbeat(this._exchangeRateApi, null, this._ruleApi, this._enrichmentApi, this._logger));
        }

        [Test]
        public void Constructor_NullRulesApi_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new ApiHeartbeat(
                    this._exchangeRateApi,
                    this._marketApi,
                    null,
                    this._enrichmentApi,
                    this._logger));
        }

        [Test]
        public async Task HeartsBeating_AllReturnFalse_IfInternalException()
        {
            var heartbeat = new ApiHeartbeat(
                this._exchangeRateApi,
                this._marketApi,
                this._ruleApi,
                this._enrichmentApi,
                this._logger);

            A.CallTo(() => this._exchangeRateApi.HeartBeating(A<CancellationToken>.Ignored))
                .Throws<ArgumentNullException>();
            A.CallTo(() => this._marketApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => this._ruleApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));

            var result = await heartbeat.HeartsBeating();

            Assert.AreEqual(result, false);
        }

        [Test]
        public async Task HeartsBeating_AllReturnTrue_IsTrue()
        {
            var heartbeat = new ApiHeartbeat(
                this._exchangeRateApi,
                this._marketApi,
                this._ruleApi,
                this._enrichmentApi,
                this._logger);

            A.CallTo(() => this._exchangeRateApi.HeartBeating(A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(true));
            A.CallTo(() => this._marketApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => this._ruleApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => this._enrichmentApi.HeartBeating(A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(true));

            var result = await heartbeat.HeartsBeating();

            Assert.AreEqual(result, true);
        }

        [Test]
        public async Task HeartsBeating_AllReturnTrueExceptExchange_IsFalse()
        {
            var heartbeat = new ApiHeartbeat(
                this._exchangeRateApi,
                this._marketApi,
                this._ruleApi,
                this._enrichmentApi,
                this._logger);

            A.CallTo(() => this._exchangeRateApi.HeartBeating(A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(false));
            A.CallTo(() => this._marketApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => this._ruleApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));

            var result = await heartbeat.HeartsBeating();

            Assert.AreEqual(result, false);
        }

        [Test]
        public async Task HeartsBeating_AllReturnTrueExceptMarket_IsFalse()
        {
            var heartbeat = new ApiHeartbeat(
                this._exchangeRateApi,
                this._marketApi,
                this._ruleApi,
                this._enrichmentApi,
                this._logger);
            A.CallTo(() => this._exchangeRateApi.HeartBeating(A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(true));
            A.CallTo(() => this._marketApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(false));
            A.CallTo(() => this._ruleApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));

            var result = await heartbeat.HeartsBeating();

            Assert.AreEqual(result, false);
        }

        [Test]
        public async Task HeartsBeating_AllReturnTrueExceptRules_IsFalse()
        {
            var heartbeat = new ApiHeartbeat(
                this._exchangeRateApi,
                this._marketApi,
                this._ruleApi,
                this._enrichmentApi,
                this._logger);

            A.CallTo(() => this._exchangeRateApi.HeartBeating(A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(true));
            A.CallTo(() => this._marketApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => this._ruleApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(false));

            var result = await heartbeat.HeartsBeating();

            Assert.AreEqual(result, false);
        }

        [SetUp]
        public void Setup()
        {
            this._exchangeRateApi = A.Fake<IExchangeRateApiCachingDecorator>();
            this._marketApi = A.Fake<IMarketOpenCloseApiCachingDecorator>();
            this._ruleApi = A.Fake<IRuleParameterApi>();
            this._enrichmentApi = A.Fake<IEnrichmentApi>();
            this._logger = A.Fake<ILogger<ApiHeartbeat>>();
        }
    }
}
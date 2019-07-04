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

namespace Surveillance.Engine.Rules.Tests.Utility
{
    [TestFixture]
    public class ApiHeartbeatTests
    {
        private IExchangeRateApiCachingDecorator _exchangeRateApi;
        private IMarketOpenCloseApiCachingDecorator _marketApi;
        private IRuleParameterApi _ruleApi;
        private IEnrichmentApi _enrichmentApi;
        private ILogger<ApiHeartbeat> _logger;

        [SetUp]
        public void Setup()
        {
            _exchangeRateApi = A.Fake<IExchangeRateApiCachingDecorator>();
            _marketApi = A.Fake<IMarketOpenCloseApiCachingDecorator>();
            _ruleApi = A.Fake<IRuleParameterApi>();
            _enrichmentApi = A.Fake<IEnrichmentApi>();
            _logger = A.Fake<ILogger<ApiHeartbeat>>();
        }

        [Test]
        public void Constructor_NullExchangeRateApi_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ApiHeartbeat(null, _marketApi, _ruleApi, _enrichmentApi, _logger));
        }

        [Test]
        public void Constructor_NullMarketApi_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ApiHeartbeat(_exchangeRateApi, null, _ruleApi, _enrichmentApi, _logger));
        }

        [Test]
        public void Constructor_NullRulesApi_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ApiHeartbeat(_exchangeRateApi, _marketApi, null, _enrichmentApi, _logger));
        }

        [Test]
        public async Task HeartsBeating_AllReturnTrue_IsTrue()
        {
            var heartbeat = new ApiHeartbeat(_exchangeRateApi, _marketApi, _ruleApi, _enrichmentApi, _logger);

            A.CallTo(() => _exchangeRateApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => _marketApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => _ruleApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => _enrichmentApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));

            var result = await heartbeat.HeartsBeating();

            Assert.AreEqual(result, true);
        }

        [Test]
        public async Task HeartsBeating_AllReturnTrueExceptExchange_IsFalse()
        {
            var heartbeat = new ApiHeartbeat(_exchangeRateApi, _marketApi, _ruleApi, _enrichmentApi, _logger);

            A.CallTo(() => _exchangeRateApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(false));
            A.CallTo(() => _marketApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => _ruleApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));

            var result = await heartbeat.HeartsBeating();

            Assert.AreEqual(result, false);
        }

        [Test]
        public async Task HeartsBeating_AllReturnTrueExceptMarket_IsFalse()
        {
            var heartbeat = new ApiHeartbeat(_exchangeRateApi, _marketApi, _ruleApi, _enrichmentApi, _logger);
            A.CallTo(() => _exchangeRateApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => _marketApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(false));
            A.CallTo(() => _ruleApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));

            var result = await heartbeat.HeartsBeating();

            Assert.AreEqual(result, false);
        }

        [Test]
        public async Task HeartsBeating_AllReturnTrueExceptRules_IsFalse()
        {
            var heartbeat = new ApiHeartbeat(_exchangeRateApi, _marketApi, _ruleApi, _enrichmentApi, _logger);

            A.CallTo(() => _exchangeRateApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => _marketApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => _ruleApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(false));

            var result = await heartbeat.HeartsBeating();

            Assert.AreEqual(result, false);
        }

        [Test]
        public async Task HeartsBeating_AllReturnFalse_IfInternalException()
        {
            var heartbeat = new ApiHeartbeat(_exchangeRateApi, _marketApi, _ruleApi, _enrichmentApi, _logger);

            A.CallTo(() => _exchangeRateApi.HeartBeating(A<CancellationToken>.Ignored)).Throws<ArgumentNullException>();
            A.CallTo(() => _marketApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => _ruleApi.HeartBeating(A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));

            var result = await heartbeat.HeartsBeating();

            Assert.AreEqual(result, false);
        }
    }
}

using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Api.Enrichment.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Utility;

namespace Surveillance.Tests.Utility
{
    [TestFixture]
    public class ApiHeartbeatTests
    {
        private IExchangeRateApiCachingDecoratorRepository _exchangeRateApi;
        private IMarketOpenCloseApiCachingDecoratorRepository _marketApi;
        private IRuleParameterApiRepository _ruleApi;
        private IEnrichmentApiRepository _enrichmentApi;
        private ILogger<ApiHeartbeat> _logger;

        [SetUp]
        public void Setup()
        {
            _exchangeRateApi = A.Fake<IExchangeRateApiCachingDecoratorRepository>();
            _marketApi = A.Fake<IMarketOpenCloseApiCachingDecoratorRepository>();
            _ruleApi = A.Fake<IRuleParameterApiRepository>();
            _enrichmentApi = A.Fake<IEnrichmentApiRepository>();
            _logger = A.Fake<ILogger<ApiHeartbeat>>();
        }

        [Test]
        public void Constructor_NullExchangeRateApi_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ApiHeartbeat(null, _marketApi, _ruleApi, _enrichmentApi, _logger));
        }

        [Test]
        public void Constructor_NullMarketApi_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ApiHeartbeat(_exchangeRateApi, null, _ruleApi, _enrichmentApi, _logger));
        }

        [Test]
        public void Constructor_NullRulesApi_IsExceptional()
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

using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Markets;
using Surveillance.Engine.Rules.Rules;

namespace Surveillance.Engine.Rules.Tests.Factories
{
    [TestFixture]
    public class UniverseMarketCacheFactoryTests
    {
        private IStubRuleRunDataRequestRepository _stubDataRequestRepository;
        private IRuleRunDataRequestRepository _dataRequestRepository;
        private ILogger<UniverseMarketCacheFactory> _logger;

        [SetUp]
        public void Setup()
        {
            _stubDataRequestRepository = A.Fake<IStubRuleRunDataRequestRepository>();
            _dataRequestRepository = A.Fake<IRuleRunDataRequestRepository>();
            _logger = new NullLogger<UniverseMarketCacheFactory>();
        }

        [Test]
        public void Constructor_Null_StubDataRequestRepository_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseMarketCacheFactory(null, _dataRequestRepository, _logger));
        }

        [Test]
        public void Constructor_Null_DataRequestRepository_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseMarketCacheFactory(_stubDataRequestRepository, null, _logger));
        }

        [Test]
        public void Constructor_Null_Logger_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseMarketCacheFactory(_stubDataRequestRepository, _dataRequestRepository, null));
        }

        [Test]
        public void BuildIntraday_Returns_IntradayCache()
        {
            var factory = new UniverseMarketCacheFactory(_stubDataRequestRepository, _dataRequestRepository, _logger);

            var result = factory.BuildIntraday(TimeSpan.FromDays(1), RuleRunMode.ForceRun);

            Assert.IsInstanceOf<UniverseEquityIntradayCache>(result);
        }

        [Test]
        public void BuildInterday_Returns_IntradayCache()
        {
            var factory = new UniverseMarketCacheFactory(_stubDataRequestRepository, _dataRequestRepository, _logger);

            var result = factory.BuildInterday(RuleRunMode.ForceRun);

            Assert.IsInstanceOf<UniverseEquityInterDayCache>(result);
        }
    }
}

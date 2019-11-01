namespace Surveillance.Engine.Rules.Tests.Factories
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
    using Surveillance.Engine.Rules.Factories;
    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Rules;

    [TestFixture]
    public class UniverseMarketCacheFactoryTests
    {
        private IRuleRunDataRequestRepository _dataRequestRepository;

        private ILogger<UniverseEquityMarketCacheFactory> _logger;

        private IStubRuleRunDataRequestRepository _stubDataRequestRepository;

        [Test]
        public void BuildInterday_Returns_IntradayCache()
        {
            var factory = new UniverseEquityMarketCacheFactory(
                this._stubDataRequestRepository,
                this._dataRequestRepository,
                this._logger);

            var result = factory.BuildInterday(RuleRunMode.ForceRun);

            Assert.IsInstanceOf<UniverseEquityInterDayCache>(result);
        }

        [Test]
        public void BuildIntraday_Returns_IntradayCache()
        {
            var factory = new UniverseEquityMarketCacheFactory(
                this._stubDataRequestRepository,
                this._dataRequestRepository,
                this._logger);

            var result = factory.BuildIntraday(TimeSpan.FromDays(1), RuleRunMode.ForceRun);

            Assert.IsInstanceOf<UniverseEquityIntraDayCache>(result);
        }

        [Test]
        public void Constructor_Null_DataRequestRepository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UniverseEquityMarketCacheFactory(this._stubDataRequestRepository, null, this._logger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UniverseEquityMarketCacheFactory(
                    this._stubDataRequestRepository,
                    this._dataRequestRepository,
                    null));
        }

        [Test]
        public void Constructor_Null_StubDataRequestRepository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UniverseEquityMarketCacheFactory(null, this._dataRequestRepository, this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._stubDataRequestRepository = A.Fake<IStubRuleRunDataRequestRepository>();
            this._dataRequestRepository = A.Fake<IRuleRunDataRequestRepository>();
            this._logger = new NullLogger<UniverseEquityMarketCacheFactory>();
        }
    }
}
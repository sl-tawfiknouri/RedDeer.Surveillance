using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    [TestFixture]
    public class LayeringFactoryTests
    {
        private IMarketTradingHoursManager _tradingHoursManager;
        private IUniverseMarketCacheFactory _factory;
        private IUniverseEquityOrderFilter _orderFilter;
        private ILogger<EquityRuleLayeringFactory> _logger;
        private ILogger<TradingHistoryStack> _tradingLogger;

        private ILayeringRuleEquitiesParameters _equitiesParameters;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        [SetUp]
        public void Setup()
        {
            _tradingHoursManager = A.Fake<IMarketTradingHoursManager>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _orderFilter = A.Fake<IUniverseEquityOrderFilter>();
            _logger = A.Fake<ILogger<EquityRuleLayeringFactory>>();
            _tradingLogger = A.Fake<ILogger<TradingHistoryStack>>();

            _equitiesParameters = A.Fake<ILayeringRuleEquitiesParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
        }

        [Test]
        public void Constructor_ConsidersNullOrderFilter_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleLayeringFactory(null, _tradingHoursManager, _factory, _logger, _tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullMarketTradingHoursManager_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleLayeringFactory(_orderFilter, null, _factory, _logger, _tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullUniverseMarketCacheFactory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleLayeringFactory(_orderFilter, _tradingHoursManager, null, _logger, _tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleLayeringFactory(_orderFilter, _tradingHoursManager, _factory, null, _tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullTradingHistoryLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleLayeringFactory(_orderFilter, _tradingHoursManager, _factory, _logger, null));
        }

        [Test]
        public void Build_Returns_Non_Null_Layering_Rule()
        {
            var ruleFactory = new EquityRuleLayeringFactory(_orderFilter, _tradingHoursManager, _factory, _logger, _tradingLogger);

            var result = ruleFactory.Build(_equitiesParameters, _ruleCtx, _alertStream, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }
    }
}

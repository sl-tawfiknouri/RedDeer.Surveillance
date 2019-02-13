using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Factories
{
    [TestFixture]
    public class LayeringFactoryTests
    {
        private IMarketTradingHoursManager _tradingHoursManager;
        private IUniverseMarketCacheFactory _factory;
        private IUniverseOrderFilter _orderFilter;
        private ILogger<LayeringRuleFactory> _logger;
        private ILogger<TradingHistoryStack> _tradingLogger;

        private ILayeringRuleParameters _parameters;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        [SetUp]
        public void Setup()
        {
            _tradingHoursManager = A.Fake<IMarketTradingHoursManager>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _orderFilter = A.Fake<IUniverseOrderFilter>();
            _logger = A.Fake<ILogger<LayeringRuleFactory>>();
            _tradingLogger = A.Fake<ILogger<TradingHistoryStack>>();

            _parameters = A.Fake<ILayeringRuleParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
        }

        [Test]
        public void Constructor_ConsidersNullOrderFilter_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRuleFactory(null, _tradingHoursManager, _factory, _logger, _tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullMarketTradingHoursManager_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRuleFactory(_orderFilter, null, _factory, _logger, _tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullUniverseMarketCacheFactory_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRuleFactory(_orderFilter, _tradingHoursManager, null, _logger, _tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRuleFactory(_orderFilter, _tradingHoursManager, _factory, null, _tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullTradingHistoryLogger_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRuleFactory(_orderFilter, _tradingHoursManager, _factory, _logger, null));
        }

        [Test]
        public void Build_Returns_Non_Null_Layering_Rule()
        {
            var ruleFactory = new LayeringRuleFactory(_orderFilter, _tradingHoursManager, _factory, _logger, _tradingLogger);

            var result = ruleFactory.Build(_parameters, _ruleCtx, _alertStream, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }
    }
}

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Tests.Factories
{
    [TestFixture]
    public class SpoofingRuleFactoryTests
    {
        private IUniverseOrderFilter _orderFilter;
        private IUniverseMarketCacheFactory _factory;
        private ILogger<SpoofingRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private ISpoofingRuleParameters _spoofingParameters;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        [SetUp]
        public void Setup()
        {
            _orderFilter = A.Fake<IUniverseOrderFilter>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _logger = new NullLogger<SpoofingRule>();
            _tradingHistoryLogger = new NullLogger<TradingHistoryStack>();

            _spoofingParameters = A.Fake<ISpoofingRuleParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
        }

        [Test]
        public void Constructor_Null_Factory_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new SpoofingRuleFactory(null, _orderFilter, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_OrderFilter_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new SpoofingRuleFactory(_factory, null, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new SpoofingRuleFactory(_factory, _orderFilter, null, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_TradingHistoryLogger_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new SpoofingRuleFactory(_factory, _orderFilter, _logger, null));
        }

        [Test]
        public void Build_Returns_Non_Null_Rule()
        {
            var factory = new SpoofingRuleFactory(_factory, _orderFilter, _logger, _tradingHistoryLogger);

            var result = factory.Build(_spoofingParameters, _ruleCtx, _alertStream, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }
    }
}

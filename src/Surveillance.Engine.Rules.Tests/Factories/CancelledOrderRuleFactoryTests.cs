using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Tests.Factories
{
    [TestFixture]
    public class CancelledOrderRuleFactoryTests
    {
        private IUniverseOrderFilter _orderFilter;
        private IUniverseMarketCacheFactory _factory;
        private ILogger<CancelledOrderRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private ICancelledOrderRuleParameters _parameters;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        [SetUp]
        public void Setup()
        {
            _orderFilter = A.Fake<IUniverseOrderFilter>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _logger = A.Fake<ILogger<CancelledOrderRule>>();
            _tradingHistoryLogger = A.Fake<ILogger<TradingHistoryStack>>();

            _parameters = A.Fake<ICancelledOrderRuleParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
        }

        [Test]
        public void Constructor_Order_Filter_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderRuleFactory(null, _factory, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Factory_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderRuleFactory(_orderFilter, null, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderRuleFactory(_orderFilter, _factory, null, _tradingHistoryLogger));
        }

        [Test]
        public void Build_Returns_A_Cancelled_Order_Rule()
        {
            var factory = new CancelledOrderRuleFactory(_orderFilter, _factory, _logger, _tradingHistoryLogger);

            var result = factory.Build(_parameters, _ruleCtx, _alertStream, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }
    }
}

using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    [TestFixture]
    public class CancelledOrderRuleFactoryTests
    {
        private IUniverseEquityOrderFilter _orderFilter;
        private IUniverseMarketCacheFactory _factory;
        private ILogger<CancelledOrderRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private ICancelledOrderRuleEquitiesParameters _parameters;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        [SetUp]
        public void Setup()
        {
            _orderFilter = A.Fake<IUniverseEquityOrderFilter>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _logger = A.Fake<ILogger<CancelledOrderRule>>();
            _tradingHistoryLogger = A.Fake<ILogger<TradingHistoryStack>>();

            _parameters = A.Fake<ICancelledOrderRuleEquitiesParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
        }

        [Test]
        public void Constructor_Order_Filter_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleCancelledOrderFactory(null, _factory, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Factory_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleCancelledOrderFactory(_orderFilter, null, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleCancelledOrderFactory(_orderFilter, _factory, null, _tradingHistoryLogger));
        }

        [Test]
        public void Build_Returns_A_Cancelled_Order_Rule()
        {
            var factory = new EquityRuleCancelledOrderFactory(_orderFilter, _factory, _logger, _tradingHistoryLogger);

            var result = factory.Build(_parameters, _ruleCtx, _alertStream, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }
    }
}

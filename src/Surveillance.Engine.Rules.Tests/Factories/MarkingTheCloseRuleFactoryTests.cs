using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.MarkingTheClose;
using Surveillance.Engine.Rules.Rules.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Factories
{
    [TestFixture]
    public class MarkingTheCloseRuleFactoryTests
    {
        private IUniverseOrderFilter _orderFilter;
        private IUniverseMarketCacheFactory _factory;
        private IMarketTradingHoursManager _tradingHoursManager;
        private ILogger<MarkingTheCloseRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private IMarkingTheCloseParameters _parameters;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        [SetUp]
        public void Setup()
        {
            _orderFilter = A.Fake<IUniverseOrderFilter>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _tradingHoursManager = A.Fake<IMarketTradingHoursManager>();
            _logger = new NullLogger<MarkingTheCloseRule>();
            _tradingHistoryLogger = new NullLogger<TradingHistoryStack>();

            _parameters = A.Fake<IMarkingTheCloseParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
        }

        [Test]
        public void Constructor_Null_OrderFilter_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarkingTheCloseRuleFactory(null, _factory, _tradingHoursManager, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Factory_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarkingTheCloseRuleFactory(_orderFilter, null, _tradingHoursManager, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_TradingHoursManager_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarkingTheCloseRuleFactory(_orderFilter, _factory, null, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarkingTheCloseRuleFactory(_orderFilter, _factory, _tradingHoursManager, null, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_TradingHistoryLogger_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarkingTheCloseRuleFactory(_orderFilter, _factory, _tradingHoursManager, _logger, null));
        }

        [Test]
        public void Build_Returns_Non_Null_Result()
        {
            var factory = new MarkingTheCloseRuleFactory(_orderFilter, _factory, _tradingHoursManager, _logger, _tradingHistoryLogger);

            var result = factory.Build(_parameters, _ruleCtx, _alertStream, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }
    }
}

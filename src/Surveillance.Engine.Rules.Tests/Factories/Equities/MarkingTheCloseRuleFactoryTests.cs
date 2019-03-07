using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    [TestFixture]
    public class MarkingTheCloseRuleFactoryTests
    {
        private IUniverseEquityOrderFilterService _orderFilterService;
        private IUniverseMarketCacheFactory _factory;
        private IMarketTradingHoursService _tradingHoursService;
        private ILogger<MarkingTheCloseRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private IMarkingTheCloseEquitiesParameters _equitiesParameters;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;
        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        [SetUp]
        public void Setup()
        {
            _orderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _tradingHoursService = A.Fake<IMarketTradingHoursService>();
            _logger = new NullLogger<MarkingTheCloseRule>();
            _tradingHistoryLogger = new NullLogger<TradingHistoryStack>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            _equitiesParameters = A.Fake<IMarkingTheCloseEquitiesParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
        }

        [Test]
        public void Constructor_Null_OrderFilter_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleMarkingTheCloseFactory(null, _factory, _tradingHoursService, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Factory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleMarkingTheCloseFactory(_orderFilterService, null, _tradingHoursService, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_TradingHoursManager_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleMarkingTheCloseFactory(_orderFilterService, _factory, null, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleMarkingTheCloseFactory(_orderFilterService, _factory, _tradingHoursService, null, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_TradingHistoryLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleMarkingTheCloseFactory(_orderFilterService, _factory, _tradingHoursService, _logger, null));
        }

        [Test]
        public void Build_Returns_Non_Null_Result()
        {
            var factory = new EquityRuleMarkingTheCloseFactory(_orderFilterService, _factory, _tradingHoursService, _logger, _tradingHistoryLogger);

            var result = factory.Build(_equitiesParameters, _ruleCtx, _alertStream, RuleRunMode.ForceRun, _dataRequestSubscriber);

            Assert.IsNotNull(result);
        }
    }
}

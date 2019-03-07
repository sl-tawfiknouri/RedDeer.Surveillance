using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    [TestFixture]
    public class HighVolumeRuleFactoryTests
    {
        private IUniverseEquityOrderFilter _orderFilter;
        private IUniverseMarketCacheFactory _factory;
        private IMarketTradingHoursService _tradingHoursService;
        private ILogger<IHighVolumeRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private IHighVolumeRuleEquitiesParameters _equitiesParameters;
        private ISystemProcessOperationRunRuleContext _opCtx;
        private IUniverseAlertStream _alertStream;
        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        [SetUp]
        public void Setup()
        {
            _orderFilter = A.Fake<IUniverseEquityOrderFilter>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _tradingHoursService = A.Fake<IMarketTradingHoursService>();
            _logger = A.Fake<ILogger<IHighVolumeRule>>();
            _tradingHistoryLogger = A.Fake<ILogger<TradingHistoryStack>>();

            _equitiesParameters = A.Fake<IHighVolumeRuleEquitiesParameters>();
            _opCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
        }

        [Test]
        public void Constructor_Null_Order_Filter_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleHighVolumeFactory(null, _factory, _tradingHoursService, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Market_Cache_Factory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleHighVolumeFactory(_orderFilter, null, _tradingHoursService, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Market_Trading_Hours_Manager_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleHighVolumeFactory(_orderFilter, _factory, null, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleHighVolumeFactory(_orderFilter, _factory, _tradingHoursService, null, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Trading_History_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleHighVolumeFactory(_orderFilter, _factory, _tradingHoursService, _logger, null));
        }

        [Test]
        public void Build_Has_Non_Null_Response()
        {
            var factory = new EquityRuleHighVolumeFactory(_orderFilter, _factory, _tradingHoursService, _logger, _tradingHistoryLogger);

            var result = factory.Build(_equitiesParameters, _opCtx, _alertStream, _dataRequestSubscriber, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }
    }
}

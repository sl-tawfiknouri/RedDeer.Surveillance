using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Data.Subscribers.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Universe.Filter.Interfaces;

namespace Surveillance.Tests.Factories
{
    [TestFixture]
    public class HighVolumeRuleFactoryTests
    {
        private IUniverseOrderFilter _orderFilter;
        private IUniverseMarketCacheFactory _factory;
        private IMarketTradingHoursManager _tradingHoursManager;
        private ILogger<IHighVolumeRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private IHighVolumeRuleParameters _parameters;
        private ISystemProcessOperationRunRuleContext _opCtx;
        private IUniverseAlertStream _alertStream;
        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        [SetUp]
        public void Setup()
        {
            _orderFilter = A.Fake<IUniverseOrderFilter>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _tradingHoursManager = A.Fake<IMarketTradingHoursManager>();
            _logger = A.Fake<ILogger<IHighVolumeRule>>();
            _tradingHistoryLogger = A.Fake<ILogger<TradingHistoryStack>>();

            _parameters = A.Fake<IHighVolumeRuleParameters>();
            _opCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
        }

        [Test]
        public void Constructor_Null_Order_Filter_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new HighVolumeRuleFactory(null, _factory, _tradingHoursManager, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Market_Cache_Factory_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new HighVolumeRuleFactory(_orderFilter, null, _tradingHoursManager, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Market_Trading_Hours_Manager_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new HighVolumeRuleFactory(_orderFilter, _factory, null, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new HighVolumeRuleFactory(_orderFilter, _factory, _tradingHoursManager, null, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Trading_History_Logger_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new HighVolumeRuleFactory(_orderFilter, _factory, _tradingHoursManager, _logger, null));
        }

        [Test]
        public void Build_Has_Non_Null_Response()
        {
            var factory = new HighVolumeRuleFactory(_orderFilter, _factory, _tradingHoursManager, _logger, _tradingHistoryLogger);

            var result = factory.Build(_parameters, _opCtx, _alertStream, _dataRequestSubscriber, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }
    }
}

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
    using Surveillance.Engine.Rules.Currency.Interfaces;

    [TestFixture]
    public class HighVolumeRuleFactoryTests
    {
        private IUniverseEquityOrderFilterService _orderFilterService;
        private IUniverseEquityMarketCacheFactory _equityFactory;
        private IUniverseFixedIncomeMarketCacheFactory _fixedIncomeFactory;
        private IMarketTradingHoursService _tradingHoursService;
        private ICurrencyConverterService currencyConverterService;
        private ILogger<IHighVolumeRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private IHighVolumeRuleEquitiesParameters _equitiesParameters;
        private ISystemProcessOperationRunRuleContext _opCtx;
        private IUniverseAlertStream _alertStream;
        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        [SetUp]
        public void Setup()
        {
            _orderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            _equityFactory = A.Fake<IUniverseEquityMarketCacheFactory>();
            _fixedIncomeFactory = A.Fake<IUniverseFixedIncomeMarketCacheFactory>();
            _tradingHoursService = A.Fake<IMarketTradingHoursService>();
            this.currencyConverterService = A.Fake<ICurrencyConverterService>();
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
            Assert.Throws<ArgumentNullException>(() => new EquityRuleHighVolumeFactory(null, _equityFactory, _fixedIncomeFactory, _tradingHoursService, this.currencyConverterService, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Market_Cache_Factory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleHighVolumeFactory(_orderFilterService, null, _fixedIncomeFactory, _tradingHoursService, this.currencyConverterService, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Market_Trading_Hours_Manager_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleHighVolumeFactory(_orderFilterService, _equityFactory, _fixedIncomeFactory, null, this.currencyConverterService, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleHighVolumeFactory(_orderFilterService, _equityFactory, _fixedIncomeFactory, _tradingHoursService, this.currencyConverterService, null, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Trading_History_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleHighVolumeFactory(_orderFilterService, _equityFactory, _fixedIncomeFactory, _tradingHoursService, this.currencyConverterService, _logger, null));
        }

        [Test]
        public void Build_Has_Non_Null_Response()
        {
            var factory = new EquityRuleHighVolumeFactory(_orderFilterService, _equityFactory, _fixedIncomeFactory, _tradingHoursService, this.currencyConverterService, _logger, _tradingHistoryLogger);

            var result = factory.Build(_equitiesParameters, _opCtx, _alertStream, _dataRequestSubscriber, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }
    }
}

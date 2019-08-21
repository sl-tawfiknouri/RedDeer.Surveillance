namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
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

    [TestFixture]
    public class HighVolumeRuleFactoryTests
    {
        private IUniverseAlertStream _alertStream;

        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private IHighVolumeRuleEquitiesParameters _equitiesParameters;

        private IUniverseMarketCacheFactory _factory;

        private ILogger<IHighVolumeRule> _logger;

        private ISystemProcessOperationRunRuleContext _opCtx;

        private IUniverseEquityOrderFilterService _orderFilterService;

        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private IMarketTradingHoursService _tradingHoursService;

        [Test]
        public void Build_Has_Non_Null_Response()
        {
            var factory = new EquityRuleHighVolumeFactory(
                this._orderFilterService,
                this._factory,
                this._tradingHoursService,
                this._logger,
                this._tradingHistoryLogger);

            var result = factory.Build(
                this._equitiesParameters,
                this._opCtx,
                this._alertStream,
                this._dataRequestSubscriber,
                RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleHighVolumeFactory(
                    this._orderFilterService,
                    this._factory,
                    this._tradingHoursService,
                    null,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Market_Cache_Factory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleHighVolumeFactory(
                    this._orderFilterService,
                    null,
                    this._tradingHoursService,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Market_Trading_Hours_Manager_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleHighVolumeFactory(
                    this._orderFilterService,
                    this._factory,
                    null,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Order_Filter_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleHighVolumeFactory(
                    null,
                    this._factory,
                    this._tradingHoursService,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Trading_History_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleHighVolumeFactory(
                    this._orderFilterService,
                    this._factory,
                    this._tradingHoursService,
                    this._logger,
                    null));
        }

        [SetUp]
        public void Setup()
        {
            this._orderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            this._factory = A.Fake<IUniverseMarketCacheFactory>();
            this._tradingHoursService = A.Fake<IMarketTradingHoursService>();
            this._logger = A.Fake<ILogger<IHighVolumeRule>>();
            this._tradingHistoryLogger = A.Fake<ILogger<TradingHistoryStack>>();

            this._equitiesParameters = A.Fake<IHighVolumeRuleEquitiesParameters>();
            this._opCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();
            this._dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
        }
    }
}
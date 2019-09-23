namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
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

    [TestFixture]
    public class MarkingTheCloseRuleFactoryTests
    {
        private IUniverseAlertStream _alertStream;

        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private IMarkingTheCloseEquitiesParameters _equitiesParameters;

        private IUniverseMarketCacheFactory _factory;

        private ILogger<MarkingTheCloseRule> _logger;

        private IUniverseEquityOrderFilterService _orderFilterService;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private IMarketTradingHoursService _tradingHoursService;

        [Test]
        public void Build_Returns_Non_Null_Result()
        {
            var factory = new EquityRuleMarkingTheCloseFactory(
                this._orderFilterService,
                this._factory,
                this._tradingHoursService,
                this._logger,
                this._tradingHistoryLogger);

            var result = factory.Build(
                this._equitiesParameters,
                this._ruleCtx,
                this._alertStream,
                RuleRunMode.ForceRun,
                this._dataRequestSubscriber);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Constructor_Null_Factory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleMarkingTheCloseFactory(
                    this._orderFilterService,
                    null,
                    this._tradingHoursService,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleMarkingTheCloseFactory(
                    this._orderFilterService,
                    this._factory,
                    this._tradingHoursService,
                    null,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_OrderFilter_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleMarkingTheCloseFactory(
                    null,
                    this._factory,
                    this._tradingHoursService,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_TradingHistoryLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleMarkingTheCloseFactory(
                    this._orderFilterService,
                    this._factory,
                    this._tradingHoursService,
                    this._logger,
                    null));
        }

        [Test]
        public void Constructor_Null_TradingHoursManager_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleMarkingTheCloseFactory(
                    this._orderFilterService,
                    this._factory,
                    null,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [SetUp]
        public void Setup()
        {
            this._orderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            this._factory = A.Fake<IUniverseMarketCacheFactory>();
            this._tradingHoursService = A.Fake<IMarketTradingHoursService>();
            this._logger = new NullLogger<MarkingTheCloseRule>();
            this._tradingHistoryLogger = new NullLogger<TradingHistoryStack>();
            this._dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            this._equitiesParameters = A.Fake<IMarkingTheCloseEquitiesParameters>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();
        }
    }
}
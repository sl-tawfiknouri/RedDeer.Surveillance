namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.WashTrade;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    [TestFixture]
    public class WashTradeRuleFactoryTests
    {
        private IUniverseAlertStream _alertStream;

        private IClusteringService _clustering;

        private ICurrencyConverterService _currencyConverterService;

        private IWashTradeRuleEquitiesParameters _equitiesParameters;

        private IUniverseEquityMarketCacheFactory _equityFactory;

        private IUniverseFixedIncomeMarketCacheFactory _fixedIncomeFactory;

        private ILogger<WashTradeRule> _logger;

        private IUniverseEquityOrderFilterService _orderFilterService;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        [Test]
        public void Build_Parameters_Null_Throws_Exception()
        {
            var factory = new EquityRuleWashTradeFactory(
                this._currencyConverterService,
                this._clustering,
                this._orderFilterService,
                this._equityFactory,
                this._fixedIncomeFactory,
                this._logger,
                this._tradingHistoryLogger);

            Assert.Throws<ArgumentNullException>(
                () => factory.Build(null, this._ruleCtx, this._alertStream, RuleRunMode.ForceRun));
        }

        [Test]
        public void Build_Returns_A_WashTrade_Rule()
        {
            var factory = new EquityRuleWashTradeFactory(
                this._currencyConverterService,
                this._clustering,
                this._orderFilterService,
                this._equityFactory,
                this._fixedIncomeFactory,
                this._logger,
                this._tradingHistoryLogger);

            var result = factory.Build(
                this._equitiesParameters,
                this._ruleCtx,
                this._alertStream,
                RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<WashTradeRule>(result);
        }

        [Test]
        public void Build_RuleCtx_Null_Throws_Exception()
        {
            var factory = new EquityRuleWashTradeFactory(
                this._currencyConverterService,
                this._clustering,
                this._orderFilterService,
                this._equityFactory,
                this._fixedIncomeFactory,
                this._logger,
                this._tradingHistoryLogger);

            Assert.Throws<ArgumentNullException>(
                () => factory.Build(this._equitiesParameters, null, this._alertStream, RuleRunMode.ForceRun));
        }

        [Test]
        public void Constructor_Clustering_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleWashTradeFactory(
                    this._currencyConverterService,
                    null,
                    this._orderFilterService,
                    this._equityFactory,
                    this._fixedIncomeFactory,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_CurrencyConverter_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleWashTradeFactory(
                    null,
                    this._clustering,
                    this._orderFilterService,
                    this._equityFactory,
                    this._fixedIncomeFactory,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Factory_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleWashTradeFactory(
                    this._currencyConverterService,
                    this._clustering,
                    this._orderFilterService,
                    null,
                    this._fixedIncomeFactory,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleWashTradeFactory(
                    this._currencyConverterService,
                    this._clustering,
                    this._orderFilterService,
                    this._equityFactory,
                    this._fixedIncomeFactory,
                    null,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_OrderFilter_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleWashTradeFactory(
                    this._currencyConverterService,
                    this._clustering,
                    null,
                    this._equityFactory,
                    this._fixedIncomeFactory,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_TradingHistoryLogger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleWashTradeFactory(
                    this._currencyConverterService,
                    this._clustering,
                    this._orderFilterService,
                    this._equityFactory,
                    this._fixedIncomeFactory,
                    this._logger,
                    null));
        }

        [SetUp]
        public void Setup()
        {
            this._currencyConverterService = A.Fake<ICurrencyConverterService>();
            this._clustering = A.Fake<IClusteringService>();
            this._orderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            this._equityFactory = A.Fake<IUniverseEquityMarketCacheFactory>();
            this._fixedIncomeFactory = A.Fake<IUniverseFixedIncomeMarketCacheFactory>();
            this._logger = new NullLogger<WashTradeRule>();
            this._tradingHistoryLogger = new NullLogger<TradingHistoryStack>();

            this._equitiesParameters = A.Fake<IWashTradeRuleEquitiesParameters>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();
        }
    }
}
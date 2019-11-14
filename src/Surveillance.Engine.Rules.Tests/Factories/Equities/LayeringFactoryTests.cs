namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    [TestFixture]
    public class LayeringFactoryTests
    {
        private IUniverseAlertStream _alertStream;

        private ILayeringRuleEquitiesParameters _equitiesParameters;

        private IUniverseEquityMarketCacheFactory _equityFactory;

        private IUniverseFixedIncomeMarketCacheFactory _fixedIncomeFactory;

        private ILogger<EquityRuleLayeringFactory> _logger;

        private IUniverseEquityOrderFilterService _orderFilterService;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        private IMarketTradingHoursService _tradingHoursService;

        private ILogger<TradingHistoryStack> _tradingLogger;

        [Test]
        public void Build_Returns_Non_Null_Layering_Rule()
        {
            var ruleFactory = new EquityRuleLayeringFactory(
                this._orderFilterService,
                this._tradingHoursService,
                this._equityFactory,
                this._fixedIncomeFactory,
                this._logger,
                this._tradingLogger);

            var result = ruleFactory.Build(
                this._equitiesParameters,
                this._ruleCtx,
                this._alertStream,
                RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleLayeringFactory(
                    this._orderFilterService,
                    this._tradingHoursService,
                    this._equityFactory,
                    this._fixedIncomeFactory,
                    null,
                    this._tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullMarketTradingHoursManager_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleLayeringFactory(
                    this._orderFilterService,
                    null,
                    this._equityFactory,
                    this._fixedIncomeFactory,
                    this._logger,
                    this._tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullOrderFilter_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleLayeringFactory(
                    null,
                    this._tradingHoursService,
                    this._equityFactory,
                    this._fixedIncomeFactory,
                    this._logger,
                    this._tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullTradingHistoryLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleLayeringFactory(
                    this._orderFilterService,
                    this._tradingHoursService,
                    this._equityFactory,
                    this._fixedIncomeFactory,
                    this._logger,
                    null));
        }

        [Test]
        public void Constructor_ConsidersNullUniverseMarketCacheFactory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleLayeringFactory(
                    this._orderFilterService,
                    this._tradingHoursService,
                    null,
                    this._fixedIncomeFactory,
                    this._logger,
                    this._tradingLogger));
        }

        [SetUp]
        public void Setup()
        {
            this._tradingHoursService = A.Fake<IMarketTradingHoursService>();
            this._equityFactory = A.Fake<IUniverseEquityMarketCacheFactory>();
            this._fixedIncomeFactory = A.Fake<IUniverseFixedIncomeMarketCacheFactory>();
            this._orderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            this._logger = A.Fake<ILogger<EquityRuleLayeringFactory>>();
            this._tradingLogger = A.Fake<ILogger<TradingHistoryStack>>();

            this._equitiesParameters = A.Fake<ILayeringRuleEquitiesParameters>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();
        }
    }
}
namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    using System;

    using Domain.Core.Trading.Execution.Interfaces;
    using Domain.Core.Trading.Factories.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.Spoofing;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    [TestFixture]
    public class SpoofingRuleFactoryTests
    {
        private IUniverseAlertStream _alertStream;

        private IUniverseEquityMarketCacheFactory _equityFactory;

        private IUniverseFixedIncomeMarketCacheFactory _fixedIncomeFactory;

        private ILogger<SpoofingRule> _logger;

        private IOrderAnalysisService _orderAnalysisService;

        private IUniverseEquityOrderFilterService _orderFilterService;

        private IPortfolioFactory _portfolioFactory;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        private ISpoofingRuleEquitiesParameters _spoofingEquitiesParameters;

        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        [Test]
        public void Build_Returns_Non_Null_Rule()
        {
            var factory = new EquityRuleSpoofingFactory(
                this._equityFactory,
                this._fixedIncomeFactory,
                this._orderFilterService,
                this._portfolioFactory,
                this._orderAnalysisService,
                this._logger,
                this._tradingHistoryLogger);

            var result = factory.Build(
                this._spoofingEquitiesParameters,
                this._ruleCtx,
                this._alertStream,
                RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Constructor_Null_Factory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleSpoofingFactory(
                    null,
                    this._fixedIncomeFactory,
                    this._orderFilterService,
                    this._portfolioFactory,
                    this._orderAnalysisService,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleSpoofingFactory(
                    this._equityFactory,
                    this._fixedIncomeFactory,
                    this._orderFilterService,
                    this._portfolioFactory,
                    this._orderAnalysisService,
                    null,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_OrderFilter_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleSpoofingFactory(
                    this._equityFactory,
                    this._fixedIncomeFactory,
                    null,
                    this._portfolioFactory,
                    this._orderAnalysisService,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_TradingHistoryLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleSpoofingFactory(
                    this._equityFactory,
                    this._fixedIncomeFactory,
                    this._orderFilterService,
                    this._portfolioFactory,
                    this._orderAnalysisService,
                    this._logger,
                    null));
        }

        [SetUp]
        public void Setup()
        {
            this._orderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            this._equityFactory = A.Fake<IUniverseEquityMarketCacheFactory>();
            this._fixedIncomeFactory = A.Fake<IUniverseFixedIncomeMarketCacheFactory>();
            this._portfolioFactory = A.Fake<IPortfolioFactory>();
            this._orderAnalysisService = A.Fake<IOrderAnalysisService>();
            this._logger = new NullLogger<SpoofingRule>();
            this._tradingHistoryLogger = new NullLogger<TradingHistoryStack>();

            this._spoofingEquitiesParameters = A.Fake<ISpoofingRuleEquitiesParameters>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();
        }
    }
}
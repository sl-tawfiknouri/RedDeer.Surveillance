namespace Surveillance.Engine.Rules.Tests.Factories.FixedIncome
{
    using System;

    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.FixedIncome;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The fixed income high profit factory tests.
    /// </summary>
    [TestFixture]
    public class FixedIncomeHighProfitFactoryTests
    {
        /// <summary>
        /// The fixed income order filter service.
        /// </summary>
        private IUniverseFixedIncomeOrderFilterService fixedIncomeOrderFilterService;

        /// <summary>
        /// The market cache factory.
        /// </summary>
        private IUniverseEquityMarketCacheFactory equityMarketCacheFactory;

        /// <summary>
        /// The market cache factory.
        /// </summary>
        private IUniverseFixedIncomeMarketCacheFactory fixedIncomeMarketCacheFactory;

        /// <summary>
        /// The market data cache strategy factory.
        /// </summary>
        private IFixedIncomeMarketDataCacheStrategyFactory marketDataCacheStrategyFactory;

        /// <summary>
        /// The cost calculator factory.
        /// </summary>
        private ICostCalculatorFactory costCalculatorFactory;

        /// <summary>
        /// The revenue calculator factory.
        /// </summary>
        private IRevenueCalculatorFactory revenueCalculatorFactory;

        /// <summary>
        /// The exchange rate profit calculator.
        /// </summary>
        private IExchangeRateProfitCalculator exchangeRateProfitCalculator;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<FixedIncomeHighProfitsRule> logger;

        /// <summary>
        /// The stack logger.
        /// </summary>
        private ILogger<TradingHistoryStack> stackLogger;

        /// <summary>
        /// The parameters.
        /// </summary>
        private IHighProfitsRuleFixedIncomeParameters parameters;

        /// <summary>
        /// The rule context.
        /// </summary>
        private ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// The judgement service.
        /// </summary>
        private IFixedIncomeHighProfitJudgementService judgementService;

        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// The currency converter service.
        /// </summary>
        private ICurrencyConverterService currencyConverterService;

        /// <summary>
        /// The run mode.
        /// </summary>
        private RuleRunMode runMode;

        /// <summary>
        /// The scheduled execution.
        /// </summary>
        private ScheduledExecution scheduledExecution;

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.fixedIncomeOrderFilterService = A.Fake<IUniverseFixedIncomeOrderFilterService>();
            this.equityMarketCacheFactory = A.Fake<IUniverseEquityMarketCacheFactory>();
            this.fixedIncomeMarketCacheFactory = A.Fake<IUniverseFixedIncomeMarketCacheFactory>();
            this.marketDataCacheStrategyFactory = A.Fake<IFixedIncomeMarketDataCacheStrategyFactory>();
            this.costCalculatorFactory = A.Fake<ICostCalculatorFactory>();
            this.revenueCalculatorFactory = A.Fake<IRevenueCalculatorFactory>();
            this.exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            this.logger = A.Fake<ILogger<FixedIncomeHighProfitsRule>>();
            this.stackLogger = A.Fake<ILogger<TradingHistoryStack>>();
            this.parameters = A.Fake<IHighProfitsRuleFixedIncomeParameters>();
            this.ruleContext = A.Fake<ISystemProcessOperationRunRuleContext>();
            this.judgementService = A.Fake<IFixedIncomeHighProfitJudgementService>();
            this.dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            this.currencyConverterService = A.Fake<ICurrencyConverterService>();
            this.runMode = RuleRunMode.ValidationRun;
            this.scheduledExecution = new ScheduledExecution();
        }

        /// <summary>
        /// The constructor_ null filter service_ throws argument exception.
        /// </summary>
        [Test]
        public void ConstructorNullFilterServiceThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(
                // ReSharper disable once ObjectCreationAsStatement
                () => new FixedIncomeHighProfitFactory(
                    null,
                    this.equityMarketCacheFactory,
                    this.fixedIncomeMarketCacheFactory,
                    this.marketDataCacheStrategyFactory,
                    this.costCalculatorFactory,
                    this.revenueCalculatorFactory,
                    this.exchangeRateProfitCalculator,
                    this.currencyConverterService,
                    this.logger,
                    this.stackLogger));
        }

        /// <summary>
        /// The constructor_ null market cache factory_ throws argument exception.
        /// </summary>
        [Test]
        public void ConstructorNullMarketCacheFactoryThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(
                // ReSharper disable once ObjectCreationAsStatement
                () => new FixedIncomeHighProfitFactory(
                    this.fixedIncomeOrderFilterService,
                    null,
                    this.fixedIncomeMarketCacheFactory,
                    this.marketDataCacheStrategyFactory,
                    this.costCalculatorFactory,
                    this.revenueCalculatorFactory,
                    this.exchangeRateProfitCalculator,
                    this.currencyConverterService,
                    this.logger,
                    this.stackLogger));
        }

        /// <summary>
        /// The constructor_ null market data cache factory_ throws argument exception.
        /// </summary>
        [Test]
        public void ConstructorNullMarketDataCacheFactoryThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(
                // ReSharper disable once ObjectCreationAsStatement
                () => new FixedIncomeHighProfitFactory(
                    this.fixedIncomeOrderFilterService,
                    this.equityMarketCacheFactory,
                    this.fixedIncomeMarketCacheFactory,
                    null,
                    this.costCalculatorFactory,
                    this.revenueCalculatorFactory,
                    this.exchangeRateProfitCalculator,
                    this.currencyConverterService,
                    this.logger,
                    this.stackLogger));
        }

        /// <summary>
        /// The constructor null cost calculator factory throws argument exception.
        /// </summary>
        [Test]
        public void ConstructorNullCostCalculatorFactoryThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(
                // ReSharper disable once ObjectCreationAsStatement
                () => new FixedIncomeHighProfitFactory(
                    this.fixedIncomeOrderFilterService,
                    this.equityMarketCacheFactory,
                    this.fixedIncomeMarketCacheFactory,
                    this.marketDataCacheStrategyFactory,
                    null,
                    this.revenueCalculatorFactory,
                    this.exchangeRateProfitCalculator,
                    this.currencyConverterService,
                    this.logger,
                    this.stackLogger));
        }

        /// <summary>
        /// The constructor null revenue calculator factory throws argument exception.
        /// </summary>
        [Test]
        public void ConstructorNullRevenueCalculatorFactoryThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(
                // ReSharper disable once ObjectCreationAsStatement
                () => new FixedIncomeHighProfitFactory(
                    this.fixedIncomeOrderFilterService,
                    this.equityMarketCacheFactory,
                    this.fixedIncomeMarketCacheFactory,
                    this.marketDataCacheStrategyFactory,
                    this.costCalculatorFactory,
                    null,
                    this.exchangeRateProfitCalculator,
                    this.currencyConverterService,
                    this.logger,
                    this.stackLogger));
        }

        /// <summary>
        /// The constructor null exchange rate profit calculator throws argument exception.
        /// </summary>
        [Test]
        public void ConstructorNullExchangeRateProfitCalculatorThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(
                // ReSharper disable once ObjectCreationAsStatement
                () => new FixedIncomeHighProfitFactory(
                    this.fixedIncomeOrderFilterService,
                    this.equityMarketCacheFactory,
                    this.fixedIncomeMarketCacheFactory,
                    this.marketDataCacheStrategyFactory,
                    this.costCalculatorFactory,
                    this.revenueCalculatorFactory,
                    null,
                    this.currencyConverterService,
                    this.logger,
                    this.stackLogger));
        }

        /// <summary>
        /// The constructor null logger throws argument exception.
        /// </summary>
        [Test]
        public void ConstructorNullLoggerThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(
                // ReSharper disable once ObjectCreationAsStatement
                () => new FixedIncomeHighProfitFactory(
                    this.fixedIncomeOrderFilterService,
                    this.equityMarketCacheFactory,
                    this.fixedIncomeMarketCacheFactory,
                    this.marketDataCacheStrategyFactory,
                    this.costCalculatorFactory,
                    this.revenueCalculatorFactory,
                    this.exchangeRateProfitCalculator,
                    this.currencyConverterService,
                    null,
                    this.stackLogger));
        }

        /// <summary>
        /// The constructor null trading history stack logger throws argument exception.
        /// </summary>
        [Test]
        public void ConstructorNullTradingHistoryStackLoggerThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(
                // ReSharper disable once ObjectCreationAsStatement
                () => new FixedIncomeHighProfitFactory(
                    this.fixedIncomeOrderFilterService,
                    this.equityMarketCacheFactory,
                    this.fixedIncomeMarketCacheFactory,
                    this.marketDataCacheStrategyFactory,
                    this.costCalculatorFactory,
                    this.revenueCalculatorFactory,
                    this.exchangeRateProfitCalculator,
                    this.currencyConverterService,
                    this.logger,
                    null));
        }

        /// <summary>
        /// The version is correct at one point zero.
        /// </summary>
        [Test]
        public void VersionIsCorrectAtOnePointZero()
        {
            var version = FixedIncomeHighProfitFactory.Version;

            Assert.AreEqual("V1.0", version);
        }

        /// <summary>
        /// The build returns rule.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// the built rule was null
        /// </exception>
        [Test]
        public void BuildReturnsRule()
        {
            var factory = this.BuildFactory();

            Assert.DoesNotThrow(
                () =>
                    {
                        var rule = factory.BuildRule(
                            this.parameters,
                            this.ruleContext,
                            this.judgementService,
                            this.dataRequestSubscriber,
                            this.runMode,
                            this.scheduledExecution);

                        if (rule == null)
                        {
                            throw new ArgumentNullException();
                        }
                    });
        }

        /// <summary>
        /// The build factory.
        /// </summary>
        /// <returns>
        /// The <see cref="FixedIncomeHighProfitFactory"/>.
        /// </returns>
        private FixedIncomeHighProfitFactory BuildFactory()
        {
            return new FixedIncomeHighProfitFactory(
                this.fixedIncomeOrderFilterService,
                this.equityMarketCacheFactory,
                this.fixedIncomeMarketCacheFactory,
                this.marketDataCacheStrategyFactory,
                this.costCalculatorFactory,
                this.revenueCalculatorFactory,
                this.exchangeRateProfitCalculator,
                this.currencyConverterService,
                this.logger,
                this.stackLogger);
        }
    }
}

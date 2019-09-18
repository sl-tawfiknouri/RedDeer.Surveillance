namespace Surveillance.Specflow.Tests.StepDefinitions.HighProfit
{
    using System;

    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora.BMLL;
    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Currency;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.ExchangeRates;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    /// <summary>
    /// The high profit steps.
    /// </summary>
    [Binding]
    public sealed class HighProfitSteps
    {
        /// <summary>
        /// The exchange rate selection.
        /// </summary>
        private readonly ExchangeRateSelection exchangeRateSelection;

        /// <summary>
        /// The scenario context.
        /// </summary>
        private readonly ScenarioContext scenarioContext;

        /// <summary>
        /// The universe selection state.
        /// </summary>
        private readonly UniverseSelectionState universeSelectionState;

        /// <summary>
        /// The cost calculator factory.
        /// </summary>
        private ICostCalculatorFactory costCalculatorFactory;

        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// The equity rule high profit factory.
        /// </summary>
        private EquityRuleHighProfitFactory equityRuleHighProfitFactory;

        /// <summary>
        /// The exchange rate profit calculator.
        /// </summary>
        private IExchangeRateProfitCalculator exchangeRateProfitCalculator;

        /// <summary>
        /// The high profit rule equities parameters.
        /// </summary>
        private HighProfitsRuleEquitiesParameters highProfitRuleEquitiesParameters;

        /// <summary>
        /// The interday universe market cache factory.
        /// </summary>
        private IUniverseMarketCacheFactory interdayUniverseMarketCacheFactory;

        /// <summary>
        /// The judgement repository.
        /// </summary>
        private IJudgementRepository judgementRepository;

        /// <summary>
        /// The judgement service.
        /// </summary>
        private JudgementService judgementService;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<HighProfitsRule> logger;

        /// <summary>
        /// The market data cache strategy factory.
        /// </summary>
        private IMarketDataCacheStrategyFactory marketDataCacheStrategyFactory;

        /// <summary>
        /// The revenue calculator factory.
        /// </summary>
        private IRevenueCalculatorFactory revenueCalculatorFactory;

        /// <summary>
        /// The rule context.
        /// </summary>
        private ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// The rule violation service.
        /// </summary>
        private IRuleViolationService ruleViolationService;

        /// <summary>
        /// The trading hours service.
        /// </summary>
        private IMarketTradingHoursService tradingHoursService;

        /// <summary>
        /// The trading logger.
        /// </summary>
        private ILogger<TradingHistoryStack> tradingLogger;

        /// <summary>
        /// The universe order filter service.
        /// </summary>
        private IUniverseEquityOrderFilterService universeOrderFilterService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighProfitSteps"/> class.
        /// </summary>
        /// <param name="scenarioContext">
        /// The scenario context.
        /// </param>
        /// <param name="universeSelectionState">
        /// The universe selection state.
        /// </param>
        /// <param name="exchangeRateSelection">
        /// The exchange rate selection.
        /// </param>
        public HighProfitSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState,
            ExchangeRateSelection exchangeRateSelection)
        {
            this.scenarioContext = scenarioContext;
            this.universeSelectionState = universeSelectionState;
            this.exchangeRateSelection = exchangeRateSelection;
        }

        /// <summary>
        /// The given i have the high profit rule parameter values.
        /// </summary>
        /// <param name="ruleParameters">
        /// The rule parameters.
        /// </param>
        [Given(@"I have the high profit rule parameter values")]
        public void GivenIHaveTheHighProfitRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                this.scenarioContext.Pending();
                return;
            }

            var parameters = ruleParameters.CreateInstance<HighProfitApiParameters>();

            this.highProfitRuleEquitiesParameters = new HighProfitsRuleEquitiesParameters(
                "0",
                TimeSpan.FromHours(parameters.WindowHours),
                TimeSpan.FromHours(parameters.FutureHours),
                parameters.PerformHighProfitWindowAnalysis,
                parameters.PerformHighProfitDailyAnalysis,
                parameters.HighProfitPercentage,
                parameters.HighProfitAbsolute,
                parameters.HighProfitUseCurrencyConversions,
                parameters.HighProfitCurrency,
                new[] { ClientOrganisationalFactors.None },
                true,
                true);
        }

        /// <summary>
        /// The then i will have alerts.
        /// </summary>
        /// <param name="alertCount">
        /// The alert count.
        /// </param>
        [Then(@"I will have (.*) high profit alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(_ => _ == alertCount);
        }

        /// <summary>
        /// The when i run the high profit rule.
        /// </summary>
        [When(@"I run the high profit rule")]
        public void WhenIRunTheHighProfitRule()
        {
            var scheduledExecution = new ScheduledExecution { IsForceRerun = true };

            this.Setup();

            var highProfitRule = this.equityRuleHighProfitFactory.Build(
                this.highProfitRuleEquitiesParameters,
                this.ruleContext,
                this.ruleContext,
                this.dataRequestSubscriber,
                this.judgementService,
                scheduledExecution);

            foreach (var universeEvent in this.universeSelectionState.SelectedUniverse.UniverseEvents)
            {
                highProfitRule.OnNext(universeEvent);
            }
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        private void Setup()
        {
            this.tradingHoursService = A.Fake<IMarketTradingHoursService>();

            A.CallTo(() => this.tradingHoursService.GetTradingHoursForMic("XLON")).Returns(
                new TradingHours
                    {
                        CloseOffsetInUtc = TimeSpan.FromHours(16),
                        IsValid = true,
                        Mic = "XLON",
                        OpenOffsetInUtc = TimeSpan.FromHours(8)
                    });

            A.CallTo(() => this.tradingHoursService.GetTradingHoursForMic("NASDAQ")).Returns(
                new TradingHours
                    {
                        CloseOffsetInUtc = TimeSpan.FromHours(23),
                        IsValid = true,
                        Mic = "NASDAQ",
                        OpenOffsetInUtc = TimeSpan.FromHours(15)
                    });

            this.interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            this.universeOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            this.logger = new NullLogger<HighProfitsRule>();
            this.tradingLogger = new NullLogger<TradingHistoryStack>();
            this.ruleContext = A.Fake<ISystemProcessOperationRunRuleContext>();
            this.dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            this.exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            this.marketDataCacheStrategyFactory = new MarketDataCacheStrategyFactory();

            this.costCalculatorFactory = new CostCalculatorFactory(
                new CurrencyConverterService(
                    this.exchangeRateSelection.ExchangeRateRepository,
                    new NullLogger<CurrencyConverterService>()),
                new NullLogger<CostCalculator>(),
                new NullLogger<CostCurrencyConvertingCalculator>());

            this.revenueCalculatorFactory = new RevenueCalculatorFactory(
                this.tradingHoursService,
                new CurrencyConverterService(
                    this.exchangeRateSelection.ExchangeRateRepository,
                    new NullLogger<CurrencyConverterService>()),
                new NullLogger<RevenueCurrencyConvertingCalculator>(),
                new NullLogger<RevenueCalculator>());

            this.equityRuleHighProfitFactory = new EquityRuleHighProfitFactory(
                this.costCalculatorFactory,
                this.revenueCalculatorFactory,
                this.exchangeRateProfitCalculator,
                this.universeOrderFilterService,
                this.interdayUniverseMarketCacheFactory,
                this.marketDataCacheStrategyFactory,
                this.logger,
                this.tradingLogger);

            this.judgementRepository = A.Fake<IJudgementRepository>();
            this.ruleViolationService = A.Fake<IRuleViolationService>();

            this.judgementService = new JudgementService(
                this.judgementRepository,
                this.ruleViolationService,
                new HighProfitJudgementMapper(new NullLogger<HighProfitJudgementMapper>()),
                new FixedIncomeHighProfitJudgementMapper(new NullLogger<FixedIncomeHighProfitJudgementMapper>()), 
                new FixedIncomeHighVolumeJudgementMapper(new NullLogger<FixedIncomeHighVolumeJudgementMapper>()), 
                new NullLogger<JudgementService>());

            this.exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            this.marketDataCacheStrategyFactory = new MarketDataCacheStrategyFactory();
        }
    }
}
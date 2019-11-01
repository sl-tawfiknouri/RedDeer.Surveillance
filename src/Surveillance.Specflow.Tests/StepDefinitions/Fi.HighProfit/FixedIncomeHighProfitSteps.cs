namespace Surveillance.Specflow.Tests.StepDefinitions.Fi.HighProfit
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
    using Surveillance.Engine.Rules.Factories.FixedIncome;
    using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.ExchangeRates;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    /// <summary>
    /// The fixed income high profit steps.
    /// </summary>
    [Binding]
    public class FixedIncomeHighProfitSteps
    {
        /// <summary>
        /// The scenario context.
        /// </summary>
        private readonly ScenarioContext scenarioContext;

        /// <summary>
        /// The universe selection state.
        /// </summary>
        private readonly UniverseSelectionState universeSelectionState;

        /// <summary>
        /// The exchange rate selection state.
        /// </summary>
        private readonly ExchangeRateSelection exchangeRateSelection;

        /// <summary>
        /// The parameters.
        /// </summary>
        private HighProfitsRuleFixedIncomeParameters parameters;

        /// <summary>
        /// The rule context for the system operation.
        /// </summary>
        private ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// The judgement service.
        /// </summary>
        private JudgementService judgementService;

        /// <summary>
        /// The judgement repository.
        /// </summary>
        private IJudgementRepository judgementRepository;

        /// <summary>
        /// The rule violation service.
        /// </summary>
        private IRuleViolationService ruleViolationService;

        /// <summary>
        /// The factory.
        /// </summary>
        private IFixedIncomeHighProfitFactory factory;

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
        /// The data request subscriber.
        /// </summary>
        private IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// The trading hours service.
        /// </summary>
        private IMarketTradingHoursService tradingHoursService;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<FixedIncomeHighProfitsRule> logger;

        /// <summary>
        /// The stack logger.
        /// </summary>
        private ILogger<TradingHistoryStack> stackLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighProfitSteps"/> class.
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
        public FixedIncomeHighProfitSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState,
            ExchangeRateSelection exchangeRateSelection)
        {
            this.scenarioContext = scenarioContext;
            this.universeSelectionState = universeSelectionState;
            this.exchangeRateSelection = exchangeRateSelection;
        }

        /// <summary>
        /// The given i have the fixed income high profit rule parameter values.
        /// </summary>
        /// <param name="ruleParameters">
        /// The rule parameters.
        /// </param>
        [Given(@"I have the fixed income high profit rule parameter values")]
        public void GivenIHaveTheFixedIncomeHighProfitRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                this.scenarioContext.Pending();
                return;
            }

            var mappedParameters = ruleParameters.CreateInstance<FixedIncomeHighProfitApiParameters>();

            this.parameters = new HighProfitsRuleFixedIncomeParameters(
                "0",
                TimeSpan.FromHours(mappedParameters.WindowHours),
                TimeSpan.FromHours(mappedParameters.FutureHours),
                mappedParameters.PerformHighProfitWindowAnalysis,
                mappedParameters.PerformHighProfitDailyAnalysis,
                mappedParameters.HighProfitPercentage,
                mappedParameters.HighProfitAbsolute,
                mappedParameters.HighProfitUseCurrencyConversions,
                mappedParameters.HighProfitCurrency,
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
        [Then(@"I will have (.*) fixed income high profit alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A
                .CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(_ => _ == alertCount);
        }

        /// <summary>
        /// The when i run the fixed income high profit rule.
        /// </summary>
        [When(@"I run the fixed income high profit rule")]
        public void WhenIRunTheFixedIncomeHighProfitRule()
        {
            var scheduledExecution = new ScheduledExecution { IsForceRerun = true };

            this.Setup();

            var rule = this.factory.BuildRule(
                this.parameters,
                this.ruleContext,
                this.judgementService,
                this.dataRequestSubscriber,
                RuleRunMode.ForceRun,
                scheduledExecution);

            foreach (var universeEvent in this.universeSelectionState.SelectedUniverse.UniverseEvents)
            {
                rule.OnNext(universeEvent);
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

            A.CallTo(() => this.tradingHoursService.GetTradingHoursForMic("Diversity")).Returns(
                new TradingHours
                    {
                        CloseOffsetInUtc = TimeSpan.FromHours(16),
                        IsValid = true,
                        Mic = "Diversity",
                        OpenOffsetInUtc = TimeSpan.FromHours(8)
                    });


            this.ruleContext = A.Fake<ISystemProcessOperationRunRuleContext>();
            this.dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            this.fixedIncomeOrderFilterService =
                new UniverseFixedIncomeOrderFilterService(
                    new NullLogger<UniverseFixedIncomeOrderFilterService>());
            
            this.logger = A.Fake<ILogger<FixedIncomeHighProfitsRule>>();
            this.stackLogger = A.Fake<ILogger<TradingHistoryStack>>();

            this.equityMarketCacheFactory = new UniverseEquityMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseEquityMarketCacheFactory>());

            this.fixedIncomeMarketCacheFactory = new UniverseFixedIncomeMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseFixedIncomeMarketCacheFactory>());

            this.marketDataCacheStrategyFactory = new FixedIncomeMarketDataCacheStrategyFactory();

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

            this.exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            this.judgementRepository = A.Fake<IJudgementRepository>();
            this.ruleViolationService = A.Fake<IRuleViolationService>();

            this.judgementService = new JudgementService(
                this.judgementRepository,
                this.ruleViolationService,
                new HighProfitJudgementMapper(new NullLogger<HighProfitJudgementMapper>()),
                new FixedIncomeHighProfitJudgementMapper(new NullLogger<FixedIncomeHighProfitJudgementMapper>()),
                new FixedIncomeHighVolumeJudgementMapper(new NullLogger<FixedIncomeHighVolumeJudgementMapper>()), 
                new NullLogger<JudgementService>());
            
            this.factory = new FixedIncomeHighProfitFactory(
                this.fixedIncomeOrderFilterService,
                this.equityMarketCacheFactory,
                this.fixedIncomeMarketCacheFactory,
                this.marketDataCacheStrategyFactory,
                this.costCalculatorFactory,
                this.revenueCalculatorFactory,
                this.exchangeRateProfitCalculator,
                this.logger,
                this.stackLogger);
        }
    }
}
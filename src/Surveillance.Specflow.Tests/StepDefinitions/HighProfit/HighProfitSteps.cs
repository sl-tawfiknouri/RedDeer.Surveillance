using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

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
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Currency;
    using Surveillance.Engine.Rules.Currency.Interfaces;
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
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.ExchangeRates;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public sealed class HighProfitSteps
    {
        private readonly ExchangeRateSelection _exchangeRateSelection;

        private readonly ScenarioContext _scenarioContext;

        private readonly UniverseSelectionState _universeSelectionState;

        private IUniverseAlertStream _alertStream;

        private ICostCalculatorFactory _costCalculatorFactory;

        private ICurrencyConverterService _currencyConverterService;

        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private EquityRuleHighProfitFactory _equityRuleHighProfitFactory;

        private IExchangeRateProfitCalculator _exchangeRateProfitCalculator;

        private HighProfitsRuleEquitiesParameters _highProfitRuleEquitiesParameters;

        private IUniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;

        private IJudgementRepository _judgementRepository;

        private JudgementService _judgementService;

        private IJudgementServiceFactory _judgementServiceFactory;

        private ILogger<HighProfitsRule> _logger;

        private IMarketDataCacheStrategyFactory _marketDataCacheStrategyFactory;

        private IRevenueCalculatorFactory _revenueCalculatorFactory;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        private IRuleViolationService _ruleViolationService;

        private IMarketTradingHoursService _tradingHoursService;

        private ILogger<TradingHistoryStack> _tradingLogger;

        private IUniverseEquityOrderFilterService _universeOrderFilterService;

        public HighProfitSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState,
            ExchangeRateSelection exchangeRateSelection)
        {
            this._scenarioContext = scenarioContext;
            this._universeSelectionState = universeSelectionState;
            this._exchangeRateSelection = exchangeRateSelection;
        }

        [Given(@"I have the high profit rule parameter values")]
        public void GivenIHaveTheHighProfitRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                this._scenarioContext.Pending();
                return;
            }

            var parameters = ruleParameters.CreateInstance<HighProfitApiParameters>();

            this._highProfitRuleEquitiesParameters = new HighProfitsRuleEquitiesParameters(
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

        [Then(@"I will have (.*) high profit alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => this._ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(_ => _ == alertCount);
        }

        [When(@"I run the high profit rule")]
        public void WhenIRunTheHighProfitRule()
        {
            var scheduledExecution = new ScheduledExecution { IsForceRerun = true };

            this.Setup();

            var highProfitRule = this._equityRuleHighProfitFactory.Build(
                this._highProfitRuleEquitiesParameters,
                this._ruleCtx,
                this._ruleCtx,
                this._dataRequestSubscriber,
                this._judgementService,
                scheduledExecution);

            foreach (var universeEvent in this._universeSelectionState.SelectedUniverse.UniverseEvents)
                highProfitRule.OnNext(universeEvent);
        }

        private void Setup()
        {
            this._tradingHoursService = A.Fake<IMarketTradingHoursService>();

            A.CallTo(() => this._tradingHoursService.GetTradingHoursForMic("XLON")).Returns(
                new TradingHours
                    {
                        CloseOffsetInUtc = TimeSpan.FromHours(16),
                        IsValid = true,
                        Mic = "XLON",
                        OpenOffsetInUtc = TimeSpan.FromHours(8)
                    });

            A.CallTo(() => this._tradingHoursService.GetTradingHoursForMic("NASDAQ")).Returns(
                new TradingHours
                    {
                        CloseOffsetInUtc = TimeSpan.FromHours(23),
                        IsValid = true,
                        Mic = "NASDAQ",
                        OpenOffsetInUtc = TimeSpan.FromHours(15)
                    });

            this._interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            var currencyLogger = new NullLogger<CurrencyConverterService>();
            this._currencyConverterService = new CurrencyConverterService(
                this._exchangeRateSelection.ExchangeRateRepository,
                currencyLogger);
            this._universeOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            this._logger = new NullLogger<HighProfitsRule>();
            this._tradingLogger = new NullLogger<TradingHistoryStack>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();
            this._dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            this._judgementServiceFactory = A.Fake<IJudgementServiceFactory>();

            this._exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            this._marketDataCacheStrategyFactory = new MarketDataCacheStrategyFactory();

            this._costCalculatorFactory = new CostCalculatorFactory(
                new CurrencyConverterService(
                    this._exchangeRateSelection.ExchangeRateRepository,
                    new NullLogger<CurrencyConverterService>()),
                new NullLogger<CostCalculator>(),
                new NullLogger<CostCurrencyConvertingCalculator>());

            this._revenueCalculatorFactory = new RevenueCalculatorFactory(
                this._tradingHoursService,
                new CurrencyConverterService(
                    this._exchangeRateSelection.ExchangeRateRepository,
                    new NullLogger<CurrencyConverterService>()),
                new NullLogger<RevenueCurrencyConvertingCalculator>(),
                new NullLogger<RevenueCalculator>());

            this._equityRuleHighProfitFactory = new EquityRuleHighProfitFactory(
                this._costCalculatorFactory,
                this._revenueCalculatorFactory,
                this._exchangeRateProfitCalculator,
                this._universeOrderFilterService,
                this._interdayUniverseMarketCacheFactory,
                this._marketDataCacheStrategyFactory,
                this._logger,
                this._tradingLogger);

            this._judgementRepository = A.Fake<IJudgementRepository>();
            this._ruleViolationService = A.Fake<IRuleViolationService>();

            this._judgementService = new JudgementService(
                this._judgementRepository,
                this._ruleViolationService,
                new HighProfitJudgementMapper(new NullLogger<HighProfitJudgementMapper>()),
                new NullLogger<JudgementService>());

            this._exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            this._marketDataCacheStrategyFactory = new MarketDataCacheStrategyFactory();
        }
    }
}
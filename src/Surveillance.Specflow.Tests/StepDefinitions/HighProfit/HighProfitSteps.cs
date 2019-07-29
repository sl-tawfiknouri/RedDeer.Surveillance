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
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.ExchangeRates;
using Surveillance.Specflow.Tests.StepDefinitions.Universe;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions.HighProfit
{
    [Binding]
    public sealed class HighProfitSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private HighProfitsRuleEquitiesParameters _highProfitRuleEquitiesParameters;
        private UniverseSelectionState _universeSelectionState;
        private ExchangeRateSelection _exchangeRateSelection;
        private JudgementService _judgementService;

        private IJudgementRepository _judgementRepository;
        private IRuleViolationService _ruleViolationService;

        private IJudgementServiceFactory _judgementServiceFactory;
        private ICurrencyConverterService _currencyConverterService;
        private IUniverseEquityOrderFilterService _universeOrderFilterService;
        private IUniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;
        private IMarketTradingHoursService _tradingHoursService;
        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private ICostCalculatorFactory _costCalculatorFactory;
        private IRevenueCalculatorFactory _revenueCalculatorFactory;
        private IExchangeRateProfitCalculator _exchangeRateProfitCalculator;
        private IMarketDataCacheStrategyFactory _marketDataCacheStrategyFactory;
        private ILogger<HighProfitsRule> _logger;
        private ILogger<TradingHistoryStack> _tradingLogger;
        private EquityRuleHighProfitFactory _equityRuleHighProfitFactory;
        
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        public HighProfitSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState, ExchangeRateSelection exchangeRateSelection)
        {
            _scenarioContext = scenarioContext;
            _universeSelectionState = universeSelectionState;
            _exchangeRateSelection = exchangeRateSelection;
        }

        private void Setup()
        {
            _tradingHoursService = A.Fake<IMarketTradingHoursService>();

            A
                .CallTo(() => _tradingHoursService.GetTradingHoursForMic("XLON"))
                .Returns(new TradingHours
                {
                    CloseOffsetInUtc = TimeSpan.FromHours(16),
                    IsValid = true,
                    Mic = "XLON",
                    OpenOffsetInUtc = TimeSpan.FromHours(8)
                });

            A
                .CallTo(() => _tradingHoursService.GetTradingHoursForMic("NASDAQ"))
                .Returns(new TradingHours
                {
                    CloseOffsetInUtc = TimeSpan.FromHours(23),
                    IsValid = true,
                    Mic = "NASDAQ",
                    OpenOffsetInUtc = TimeSpan.FromHours(15)
                });

            _interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            var currencyLogger = new NullLogger<CurrencyConverterService>();
            _currencyConverterService = new CurrencyConverterService(_exchangeRateSelection.ExchangeRateRepository, currencyLogger);
            _universeOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            _logger = new NullLogger<HighProfitsRule>();
            _tradingLogger = new NullLogger<TradingHistoryStack>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            _judgementServiceFactory = A.Fake<IJudgementServiceFactory>();

            _exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            _marketDataCacheStrategyFactory = new MarketDataCacheStrategyFactory();

            _costCalculatorFactory = new CostCalculatorFactory(
                new CurrencyConverterService(_exchangeRateSelection.ExchangeRateRepository, new NullLogger<CurrencyConverterService>()),
                new NullLogger<CostCalculator>(),
                new NullLogger<CostCurrencyConvertingCalculator>());

            _revenueCalculatorFactory =
                new RevenueCalculatorFactory(
                    _tradingHoursService,
                    new CurrencyConverterService(
                        _exchangeRateSelection.ExchangeRateRepository,
                        new NullLogger<CurrencyConverterService>()),
                    new NullLogger<RevenueCurrencyConvertingCalculator>(),
                    new NullLogger<RevenueCalculator>());
            
            _equityRuleHighProfitFactory = new EquityRuleHighProfitFactory(
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _exchangeRateProfitCalculator,
                _universeOrderFilterService,
                _interdayUniverseMarketCacheFactory,
                _marketDataCacheStrategyFactory,
                _logger,
                _tradingLogger);

            _judgementRepository = A.Fake<IJudgementRepository>();
            _ruleViolationService = A.Fake<IRuleViolationService>();

            _judgementService =
                new JudgementService(
                    _judgementRepository,
                    _ruleViolationService,
                    new HighProfitJudgementMapper(new NullLogger<HighProfitJudgementMapper>()),
                    new NullLogger<JudgementService>());

            _exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            _marketDataCacheStrategyFactory = new MarketDataCacheStrategyFactory();
        }

        [Given(@"I have the high profit rule parameter values")]
        public void GivenIHaveTheHighProfitRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                _scenarioContext.Pending();
                return;
            }

            var parameters = ruleParameters.CreateInstance<HighProfitApiParameters>();

            _highProfitRuleEquitiesParameters = new HighProfitsRuleEquitiesParameters(
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

        [When(@"I run the high profit rule")]
        public void WhenIRunTheHighProfitRule()
        {
            var scheduledExecution = new ScheduledExecution { IsForceRerun = true };
            
            Setup();

            var highProfitRule =
                _equityRuleHighProfitFactory.Build(
                    _highProfitRuleEquitiesParameters,
                    _ruleCtx,
                    _ruleCtx,
                    _dataRequestSubscriber,
                    _judgementService,
                    scheduledExecution);

            foreach (var universeEvent in _universeSelectionState.SelectedUniverse.UniverseEvents)
                highProfitRule.OnNext(universeEvent);
        }

        [Then(@"I will have (.*) high profit alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => _ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(_ => _ == alertCount);
        }
    }
}

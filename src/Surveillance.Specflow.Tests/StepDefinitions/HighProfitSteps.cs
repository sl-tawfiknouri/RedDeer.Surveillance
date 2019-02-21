using System;
using Domain.Equity.Streams.Interfaces;
using Domain.Scheduling;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Currency;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Multiverse;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.HighProfit;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions
{
    [Binding]
    public sealed class HighProfitSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private HighProfitsRuleParameters _highProfitRuleParameters;
        private UniverseSelectionState _universeSelectionState;
        private ExchangeRateSelection _exchangeRateSelection;

        private ICurrencyConverter _currencyConverter;
        private IUniverseEquityOrderFilter _universeOrderFilter;
        private IUniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;
        private IMarketTradingHoursManager _tradingHoursManager;
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
            _tradingHoursManager = A.Fake<IMarketTradingHoursManager>();

            A
                .CallTo(() => _tradingHoursManager.GetTradingHoursForMic("XLON"))
                .Returns(new TradingHours
                {
                    CloseOffsetInUtc = TimeSpan.FromHours(16),
                    IsValid = true,
                    Mic = "XLON",
                    OpenOffsetInUtc = TimeSpan.FromHours(8)
                });

            A
                .CallTo(() => _tradingHoursManager.GetTradingHoursForMic("NASDAQ"))
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

            var currencyLogger = new NullLogger<CurrencyConverter>();
            _currencyConverter = new CurrencyConverter(_exchangeRateSelection.ExchangeRateRepository, currencyLogger);
            _universeOrderFilter = A.Fake<IUniverseEquityOrderFilter>();
            _logger = new NullLogger<HighProfitsRule>();
            _tradingLogger = new NullLogger<TradingHistoryStack>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            _costCalculatorFactory = new CostCalculatorFactory(
                new CurrencyConverter(_exchangeRateSelection.ExchangeRateRepository, new NullLogger<CurrencyConverter>()),
                new NullLogger<CostCalculator>(),
                new NullLogger<CostCurrencyConvertingCalculator>());

            _revenueCalculatorFactory =
                new RevenueCalculatorFactory(
                    _tradingHoursManager,
                    new CurrencyConverter(
                        _exchangeRateSelection.ExchangeRateRepository,
                        new NullLogger<CurrencyConverter>()),
                    new NullLogger<RevenueCurrencyConvertingCalculator>(),
                    new NullLogger<RevenueCalculator>());

            _exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            _marketDataCacheStrategyFactory = new MarketDataCacheStrategyFactory();

            _equityRuleHighProfitFactory = new EquityRuleHighProfitFactory(
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _exchangeRateProfitCalculator,
                _universeOrderFilter,
                _interdayUniverseMarketCacheFactory,
                _marketDataCacheStrategyFactory,
                _logger,
                _tradingLogger);
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

            _highProfitRuleParameters = new HighProfitsRuleParameters(
                "0",
                TimeSpan.FromHours(parameters.WindowHours),
                parameters.HighProfitPercentage,
                parameters.HighProfitAbsolute,
                parameters.HighProfitUseCurrencyConversions,
                parameters.HighProfitCurrency,
                new[] { ClientOrganisationalFactors.None },
                true);
        }

        [When(@"I run the high profit rule")]
        public void WhenIRunTheHighProfitRule()
        {
            var scheduledExecution = new ScheduledExecution { IsForceRerun = true };

            Setup();

            var highProfitRule =
                _equityRuleHighProfitFactory.Build(
                    _highProfitRuleParameters,
                    _ruleCtx,
                    _ruleCtx,
                    _alertStream,
                    _dataRequestSubscriber,
                    scheduledExecution);

            foreach (var universeEvent in _universeSelectionState.SelectedUniverse.UniverseEvents)
                highProfitRule.OnNext(universeEvent);
        }

        [Then(@"I will have (.*) high profit alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.That.Matches(i => !i.IsDeleteEvent && !i.IsRemoveEvent))).MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }
    }
}

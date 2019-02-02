using System;
using System.Collections.Generic;
using DomainV2.Equity.Streams.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using RedDeer.Contracts.SurveillanceService.Rules;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Currency;
using Surveillance.Currency.Interfaces;
using Surveillance.Data.Subscribers.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets;
using Surveillance.Markets.Interfaces;
using Surveillance.RuleParameters;
using Surveillance.RuleParameters.OrganisationalFactors;
using Surveillance.Rules.HighProfits;
using Surveillance.Rules.HighProfits.Calculators;
using Surveillance.Rules.HighProfits.Calculators.Factories;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.HighProfit;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.Multiverse;
using Surveillance.Universe.Subscribers.Interfaces;
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

        private ICurrencyConverter _currencyConverter;
        private IUniverseOrderFilter _universeOrderFilter;
        private IUniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;
        private IMarketTradingHoursManager _tradingHoursManager;
        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private IUnsubscriberFactory<IUniverseEvent> _unsubscriberFactory;
        private ICostCalculatorFactory _costCalculatorFactory;
        private IRevenueCalculatorFactory _revenueCalculatorFactory;
        private IExchangeRateProfitCalculator _exchangeRateProfitCalculator;
        private IUniversePercentageCompletionLoggerFactory _percentageCompletionLogger;
        private ILogger<HighProfitsRule> _logger;
        private ILogger<TradingHistoryStack> _tradingLogger;
        private ILogger<MarketCloseMultiverseTransformer> _multiverseLogger;
        private HighProfitRuleFactory _highProfitRuleFactory;
        
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        public HighProfitSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
        {
            _scenarioContext = scenarioContext;
            _universeSelectionState = universeSelectionState;

            var exchangeRateApiRepository = A.Fake<IExchangeRateApiCachingDecoratorRepository>();

            var exchangeRateDto = new ExchangeRateDto
            {
                DateTime = new DateTime(2018, 01, 01),
                Name = "GBX/USD",
                FixedCurrency = "GBX",
                VariableCurrency = "USD",
                Rate = 200d
            };

            A.CallTo(() =>
                    exchangeRateApiRepository.Get(A<DateTime>.Ignored, A<DateTime>.Ignored))
                .Returns(new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
                        {
                            { new DateTime(2018, 01, 01), new ExchangeRateDto[] { exchangeRateDto }}
                        });

            _tradingHoursManager = A.Fake<IMarketTradingHoursManager>();

            A
                .CallTo(() => _tradingHoursManager.Get("XLON"))
                .Returns(new TradingHours
                {
                    CloseOffsetInUtc = TimeSpan.FromHours(16),
                    IsValid = true,
                    Mic = "XLON",
                    OpenOffsetInUtc = TimeSpan.FromHours(8)
                });

            _interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            var currencyLogger = new NullLogger<CurrencyConverter>();
            _currencyConverter = new CurrencyConverter(exchangeRateApiRepository, currencyLogger);
            _universeOrderFilter = A.Fake<IUniverseOrderFilter>();
            _logger = new NullLogger<HighProfitsRule>();
            _tradingLogger = new NullLogger<TradingHistoryStack>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            _unsubscriberFactory = A.Fake<IUnsubscriberFactory<IUniverseEvent>>();

            _costCalculatorFactory = new CostCalculatorFactory(
                new CurrencyConverter(exchangeRateApiRepository, new NullLogger<CurrencyConverter>()),
                new NullLogger<CostCalculator>(),
                new NullLogger<CostCurrencyConvertingCalculator>());

            _revenueCalculatorFactory =
                new RevenueCalculatorFactory(
                    _tradingHoursManager,
                    new CurrencyConverter(
                        exchangeRateApiRepository,
                        new NullLogger<CurrencyConverter>()),
                    new NullLogger<RevenueCurrencyConvertingCalculator>(),
                    new NullLogger<RevenueCalculator>());

            _exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            _percentageCompletionLogger = A.Fake<IUniversePercentageCompletionLoggerFactory>();
            _multiverseLogger = A.Fake<Logger<MarketCloseMultiverseTransformer>>();

            _highProfitRuleFactory = new HighProfitRuleFactory(
                _unsubscriberFactory,
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _exchangeRateProfitCalculator,
                _percentageCompletionLogger,
                _universeOrderFilter,
                _interdayUniverseMarketCacheFactory,
                _logger,
                _tradingLogger,
                _multiverseLogger);
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
            var scheduledExecution = new DomainV2.Scheduling.ScheduledExecution { IsForceRerun = true };

            var highProfitRule =
                _highProfitRuleFactory.Build(
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
            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }
    }
}

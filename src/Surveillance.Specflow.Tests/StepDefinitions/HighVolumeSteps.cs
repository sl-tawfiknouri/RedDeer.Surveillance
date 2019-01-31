using System;
using System.Collections.Generic;
using Accord.Statistics;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Currency;
using Surveillance.Currency.Interfaces;
using Surveillance.Data.Subscribers.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.RuleParameters;
using Surveillance.RuleParameters.OrganisationalFactors;
using Surveillance.Rules.HighVolume;
using Surveillance.Specflow.Tests.StepDefinitions.HighVolume;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Universe.Filter.Interfaces;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions
{
    [Binding]
    public sealed class HighVolumeSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private HighVolumeRuleParameters _highVolumeRuleParameters;
        private UniverseSelectionState _universeSelectionState;

        // wash trade factory and arguments
        private ICurrencyConverter _currencyConverter;

        private IUniverseOrderFilter _universeOrderFilter;
        private IUniverseMarketCacheFactory _universeMarketCacheFactory;
        private IMarketTradingHoursManager _tradingHoursManager;
        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private ILogger<HighVolumeRule> _logger;
        private ILogger<TradingHistoryStack> _tradingLogger;
        private HighVolumeRuleFactory _highVolumeRuleFactory;

        // wash trade run
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        public HighVolumeSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
        {
            _scenarioContext = scenarioContext;
            _universeSelectionState = universeSelectionState;

            var exchangeRateApiRepository = A.Fake<IExchangeRateApiCachingDecoratorRepository>();

            var exchangeRateDto = new ExchangeRateDto
            {
                DateTime = new DateTime(2018, 01, 01), Name = "GBX/USD", FixedCurrency = "GBX", VariableCurrency = "USD", Rate = 200d
            };

            A.CallTo(() =>
                    exchangeRateApiRepository.Get(A<DateTime>.Ignored, A<DateTime>.Ignored))
                .Returns(new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
                        {
                            { new DateTime(2018, 01, 01), new ExchangeRateDto[] { exchangeRateDto }}
                        });

            var currencyLogger = new NullLogger<CurrencyConverter>();
            _currencyConverter = new CurrencyConverter(exchangeRateApiRepository, currencyLogger);
            _universeOrderFilter = A.Fake<IUniverseOrderFilter>();
            _universeMarketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            _logger = new NullLogger<HighVolumeRule>();
            _tradingLogger = new NullLogger<TradingHistoryStack>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _tradingHoursManager = A.Fake<IMarketTradingHoursManager>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            _highVolumeRuleFactory = new HighVolumeRuleFactory(
                _universeOrderFilter,
                _universeMarketCacheFactory,
                _tradingHoursManager,
                _logger,
                _tradingLogger);
        }

        [Given(@"I have the high volume rule parameter values")]
        public void GivenIHaveTheHighVolumeRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                _scenarioContext.Pending();
                return;
            }

            var parameters = ruleParameters.CreateInstance<HighVolumeApiParameters>();

            _highVolumeRuleParameters = new HighVolumeRuleParameters(
                "0",
                TimeSpan.FromHours(parameters.WindowHours),
                parameters.HighVolumePercentageDaily,
                parameters.HighVolumePercentageWindow,
                parameters.HighVolumePercentageMarketCap,
                new [] {ClientOrganisationalFactors.None},
                true);
        }

        [When(@"I run the high volume rule")]
        public void WhenIRunTheHighVolumeRule()
        {
            var highVolumeRule =
                _highVolumeRuleFactory.Build(
                    _highVolumeRuleParameters,
                    _ruleCtx,
                    _alertStream,
                    _dataRequestSubscriber,
                    Rules.RuleRunMode.ForceRun);

            foreach (var universeEvent in _universeSelectionState.SelectedUniverse.UniverseEvents)
                highVolumeRule.OnNext(universeEvent);
        }

        [Then(@"I will have (.*) high volume alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }
    }
}

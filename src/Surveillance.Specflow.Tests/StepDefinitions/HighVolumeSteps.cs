using System;
using System.Collections.Generic;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
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
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.HighVolume;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions
{
    [Binding]
    public sealed class HighVolumeSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private HighVolumeRuleEquitiesParameters _highVolumeRuleEquitiesParameters;
        private UniverseSelectionState _universeSelectionState;

        private ICurrencyConverter _currencyConverter;
        private IUniverseEquityOrderFilter _universeOrderFilter;
        private IUniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;
        private IMarketTradingHoursManager _tradingHoursManager;
        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private ILogger<HighVolumeRule> _logger;
        private ILogger<TradingHistoryStack> _tradingLogger;
        private EquityRuleHighVolumeFactory _equityRuleHighVolumeFactory;

        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        public HighVolumeSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
        {
            _scenarioContext = scenarioContext;
            _universeSelectionState = universeSelectionState;

            var exchangeRateApiRepository = A.Fake<IExchangeRateApiCachingDecoratorRepository>();

            var exchangeRateDto = new ExchangeRateDto
            {
                DateTime = new DateTime(2018, 01, 01), Name = "GBX/USD", FixedCurrency = "GBX", VariableCurrency = "USD", Rate = 0.02d
            };

            A.CallTo(() =>
                    exchangeRateApiRepository.Get(A<DateTime>.Ignored, A<DateTime>.Ignored))
                .Returns(new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
                        {
                            { new DateTime(2018, 01, 01), new ExchangeRateDto[] { exchangeRateDto }}
                        });


            var repository = A.Fake<IMarketOpenCloseApiCachingDecoratorRepository>();

            A
                .CallTo(() => repository.Get()).
                Returns(new ExchangeDto[]
                {
                    new ExchangeDto
                    {
                        Code = "XLON",
                        MarketOpenTime = TimeSpan.FromHours(8),
                        MarketCloseTime = TimeSpan.FromHours(16),
                        IsOpenOnMonday = true,
                        IsOpenOnTuesday = true,
                        IsOpenOnWednesday = true,
                        IsOpenOnThursday = true,
                        IsOpenOnFriday = true,
                        IsOpenOnSaturday = true,
                        IsOpenOnSunday = true,
                    },

                    new ExchangeDto
                    {
                        Code = "NASDAQ",
                        MarketOpenTime = TimeSpan.FromHours(15),
                        MarketCloseTime = TimeSpan.FromHours(23),
                        IsOpenOnMonday = true,
                        IsOpenOnTuesday = true,
                        IsOpenOnWednesday = true,
                        IsOpenOnThursday = true,
                        IsOpenOnFriday = true,
                        IsOpenOnSaturday = true,
                        IsOpenOnSunday = true,
                    }
                });

            _tradingHoursManager = new MarketTradingHoursManager(repository, new NullLogger<MarketTradingHoursManager>());          

            _interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            var currencyLogger = new NullLogger<CurrencyConverter>();
            _currencyConverter = new CurrencyConverter(exchangeRateApiRepository, currencyLogger);
            _universeOrderFilter = A.Fake<IUniverseEquityOrderFilter>();
            _logger = new NullLogger<HighVolumeRule>();
            _tradingLogger = new NullLogger<TradingHistoryStack>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            _equityRuleHighVolumeFactory = new EquityRuleHighVolumeFactory(
                _universeOrderFilter,
                _interdayUniverseMarketCacheFactory,
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

            _highVolumeRuleEquitiesParameters = new HighVolumeRuleEquitiesParameters(
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
                _equityRuleHighVolumeFactory.Build(
                    _highVolumeRuleEquitiesParameters,
                    _ruleCtx,
                    _alertStream,
                    _dataRequestSubscriber,
                    RuleRunMode.ForceRun);

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

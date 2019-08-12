using System;
using System.Collections.Generic;
using Domain.Surveillance.Streams.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe;
using Surveillance.Engine.Rules.Universe.Filter;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;
using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.Universe;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions.HighVolumeVenueFilter
{
    [Binding]
    public sealed class HighVolumeVenueFilterSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private HighVolumeRuleEquitiesParameters _highVolumeRuleEquitiesParameters;
        private UniverseSelectionState _universeSelectionState;
        private HighVolumeVenueFilterApiParameters _venueFilterApiParameters;

        private IUniverseEquityOrderFilterService _universeOrderFilterService;
        private IUniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;
        private IUniverseFilterService _baseUniverseFilterService;
        private IUniverseDataRequestsSubscriber _universeDataRequestsSubscriber;
        private IMarketTradingHoursService _tradingHoursService;
        private ILogger<HighVolumeRule> _logger;
        private ILogger<TradingHistoryStack> _tradingLogger;

        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IObserver<IUniverseEvent> _observer;
        
        private Engine.Rules.Universe.Filter.HighVolumeVenueFilter _filter;
        private TimeWindows _timeWindows;
        private decimal? _filterMax;
        private decimal? _filterMin;
        
        public HighVolumeVenueFilterSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
        {
            _scenarioContext = scenarioContext;
            _universeSelectionState = universeSelectionState;
            _observer = A.Fake<IObserver<IUniverseEvent>>();
            _universeDataRequestsSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            var exchangeRateApiRepository = A.Fake<IExchangeRateApiCachingDecorator>();
            
            var exchangeRateDto = new ExchangeRateDto
            {
                DateTime = new DateTime(2018, 01, 01),
                Name = "GBX/USD",
                FixedCurrency = "GBX",
                VariableCurrency = "USD",
                Rate = 0.02d
            };

            A.CallTo(() =>
                    exchangeRateApiRepository.Get(A<DateTime>.Ignored, A<DateTime>.Ignored))
                .Returns(new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
                        {
                            { new DateTime(2018, 01, 01), new ExchangeRateDto[] { exchangeRateDto }}
                        });


            var repository = A.Fake<IMarketOpenCloseApiCachingDecorator>();

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

            _tradingHoursService = new MarketTradingHoursService(repository, new NullLogger<MarketTradingHoursService>());

            _interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            _universeOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            _logger = new NullLogger<HighVolumeRule>();
            _tradingLogger = new NullLogger<TradingHistoryStack>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
        }

        [Given(@"I have the high volume venue filter parameter values")]
        public void GivenIHaveTheHighVolumeVenueFilterParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                _scenarioContext.Pending();
                return;
            }

            _venueFilterApiParameters = ruleParameters.CreateInstance<HighVolumeVenueFilterApiParameters>();
            _timeWindows = new TimeWindows("some-id", TimeSpan.FromHours(_venueFilterApiParameters.WindowHours));
        }

        [When(@"I run the high volume venue filter")]
        public void WhenIRunTheHighVolumeVenueFilter()
        {
            _filter =
                new Engine.Rules.Universe.Filter.HighVolumeVenueFilter(
                    _timeWindows,
                    new DecimalRangeRuleFilter()
                    {
                        Max = _venueFilterApiParameters.Max,
                        Min = _venueFilterApiParameters.Min,
                        Type = RuleFilterType.Include
                    },
                    new UniverseEquityOrderFilterService(new NullLogger<UniverseEquityOrderFilterService>()),
                    _ruleCtx,
                    new UniverseMarketCacheFactory(
                        new StubRuleRunDataRequestRepository(),
                        new RuleRunDataRequestRepository(
                            A.Fake<IConnectionStringFactory>(),
                            new NullLogger<RuleRunDataRequestRepository>()),
                        new NullLogger<UniverseMarketCacheFactory>()),
                    RuleRunMode.ValidationRun,
                    _tradingHoursService,
                    _universeDataRequestsSubscriber,
                    DataSource.AllIntraday,
                    new NullLogger<TradingHistoryStack>(),
                    new NullLogger<Engine.Rules.Universe.Filter.HighVolumeVenueFilter>());

            _baseUniverseFilterService = A.Fake<IUniverseFilterService>();

            var filterDecorator =
                new HighVolumeVenueDecoratorFilter(
                    _timeWindows,
                    _baseUniverseFilterService,
                    _filter);

            filterDecorator.Subscribe(_observer);

            foreach (var universeEvent in _universeSelectionState.SelectedUniverse.UniverseEvents)
                filterDecorator.OnNext(universeEvent);
        }

        [Then(@"I will have (.*) filter passed orders")]
        public void ThenIWillHavePassedOrders(int expectedPassedItemCount)
        {
            A
                .CallTo(() => _baseUniverseFilterService.OnNext(A<IUniverseEvent>.That.Matches(_ => _.StateChange.IsOrderType())))
                .MustHaveHappenedANumberOfTimesMatching(_ => _ == expectedPassedItemCount);
        }
    }
}

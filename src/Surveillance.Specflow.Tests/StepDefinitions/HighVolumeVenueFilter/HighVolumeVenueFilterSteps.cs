namespace Surveillance.Specflow.Tests.StepDefinitions.HighVolumeVenueFilter
{
    using System;
    using System.Collections.Generic;

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

    [Binding]
    public sealed class HighVolumeVenueFilterSteps
    {
        private readonly IObserver<IUniverseEvent> _observer;

        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        private readonly ScenarioContext _scenarioContext;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private readonly IUniverseDataRequestsSubscriber _universeDataRequestsSubscriber;

        private readonly UniverseSelectionState _universeSelectionState;

        private IUniverseFilterService _baseUniverseFilterService;

        private HighVolumeVenueFilter _filter;

        private decimal? _filterMax;

        private decimal? _filterMin;

        private HighVolumeRuleEquitiesParameters _highVolumeRuleEquitiesParameters;

        private IUniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;

        private ILogger<HighVolumeRule> _logger;

        private TimeWindows _timeWindows;

        private ILogger<TradingHistoryStack> _tradingLogger;

        private IUniverseEquityOrderFilterService _universeOrderFilterService;

        private HighVolumeVenueFilterApiParameters _venueFilterApiParameters;

        public HighVolumeVenueFilterSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState)
        {
            this._scenarioContext = scenarioContext;
            this._universeSelectionState = universeSelectionState;
            this._observer = A.Fake<IObserver<IUniverseEvent>>();
            this._universeDataRequestsSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            var exchangeRateApiRepository = A.Fake<IExchangeRateApiCachingDecorator>();

            var exchangeRateDto = new ExchangeRateDto
                                      {
                                          DateTime = new DateTime(2018, 01, 01),
                                          Name = "GBX/USD",
                                          FixedCurrency = "GBX",
                                          VariableCurrency = "USD",
                                          Rate = 0.02d
                                      };

            A.CallTo(() => exchangeRateApiRepository.GetAsync(A<DateTime>.Ignored, A<DateTime>.Ignored)).Returns(
                new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
                    {
                        { new DateTime(2018, 01, 01), new[] { exchangeRateDto } }
                    });

            var repository = A.Fake<IMarketOpenCloseApiCachingDecorator>();

            A.CallTo(() => repository.GetAsync()).Returns(
                new[]
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
                                IsOpenOnSunday = true
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
                                IsOpenOnSunday = true
                            }
                    });

            this._tradingHoursService = new MarketTradingHoursService(
                repository,
                new NullLogger<MarketTradingHoursService>());

            this._interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            this._universeOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            this._logger = new NullLogger<HighVolumeRule>();
            this._tradingLogger = new NullLogger<TradingHistoryStack>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
        }

        [Given(@"I have the high volume venue filter parameter values")]
        public void GivenIHaveTheHighVolumeVenueFilterParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                this._scenarioContext.Pending();
                return;
            }

            this._venueFilterApiParameters = ruleParameters.CreateInstance<HighVolumeVenueFilterApiParameters>();
            this._timeWindows = new TimeWindows(
                "some-id",
                TimeSpan.FromHours(this._venueFilterApiParameters.WindowHours));
        }

        [Then(@"I will have (.*) filter passed orders")]
        public void ThenIWillHavePassedOrders(int expectedPassedItemCount)
        {
            A.CallTo(
                    () => this._baseUniverseFilterService.OnNext(
                        A<IUniverseEvent>.That.Matches(_ => _.StateChange.IsOrderType())))
                .MustHaveHappenedANumberOfTimesMatching(_ => _ == expectedPassedItemCount);
        }

        [When(@"I run the high volume venue filter")]
        public void WhenIRunTheHighVolumeVenueFilter()
        {
            this._filter = new HighVolumeVenueFilter(
                this._timeWindows,
                new DecimalRangeRuleFilter
                    {
                        Max = this._venueFilterApiParameters.Max,
                        Min = this._venueFilterApiParameters.Min,
                        Type = RuleFilterType.Include
                    },
                new UniverseEquityOrderFilterService(new NullLogger<UniverseEquityOrderFilterService>()),
                this._ruleCtx,
                new UniverseMarketCacheFactory(
                    new StubRuleRunDataRequestRepository(),
                    new RuleRunDataRequestRepository(
                        A.Fake<IConnectionStringFactory>(),
                        new NullLogger<RuleRunDataRequestRepository>()),
                    new NullLogger<UniverseMarketCacheFactory>()),
                RuleRunMode.ValidationRun,
                this._tradingHoursService,
                this._universeDataRequestsSubscriber,
                DataSource.AllIntraday,
                new NullLogger<TradingHistoryStack>(),
                new NullLogger<HighVolumeVenueFilter>());

            this._baseUniverseFilterService = A.Fake<IUniverseFilterService>();

            var filterDecorator = new HighVolumeVenueDecoratorFilter(
                this._timeWindows,
                this._baseUniverseFilterService,
                this._filter);

            filterDecorator.Subscribe(this._observer);

            foreach (var universeEvent in this._universeSelectionState.SelectedUniverse.UniverseEvents)
                filterDecorator.OnNext(universeEvent);
        }
    }
}
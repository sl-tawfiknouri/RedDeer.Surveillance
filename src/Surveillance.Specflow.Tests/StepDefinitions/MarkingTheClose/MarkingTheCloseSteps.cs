namespace Surveillance.Specflow.Tests.StepDefinitions.MarkingTheClose
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging.Abstractions;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora.BMLL;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public sealed class MarkingTheCloseSteps
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private readonly IEquityRuleMarkingTheCloseFactory _equityRuleMarkingTheCloseFactory;

        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        private readonly ScenarioContext _scenarioContext;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private readonly UniverseMarketCacheFactory _universeMarketCacheFactory;

        private readonly IUniverseEquityOrderFilterService _universeOrderFilterService;

        private readonly UniverseSelectionState _universeSelectionState;

        private MarkingTheCloseEquitiesParameters _equitiesParameters;

        public MarkingTheCloseSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
        {
            this._scenarioContext = scenarioContext;
            this._universeSelectionState = universeSelectionState;

            this._universeMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            this._alertStream = A.Fake<IUniverseAlertStream>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._universeOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();

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
            this._dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            this._equityRuleMarkingTheCloseFactory = new EquityRuleMarkingTheCloseFactory(
                this._universeOrderFilterService,
                this._universeMarketCacheFactory,
                this._tradingHoursService,
                new NullLogger<MarkingTheCloseRule>(),
                new NullLogger<TradingHistoryStack>());
        }

        [Given(@"I have the marking the close rule parameter values")]
        public void GivenIHaveTheMarkingTheCloseRuleParameterValues(Table markingTheCloseParameters)
        {
            if (markingTheCloseParameters.RowCount != 1)
            {
                this._scenarioContext.Pending();
                return;
            }

            var parameters = markingTheCloseParameters.CreateInstance<MarkingTheCloseApiParameters>();

            this._equitiesParameters = new MarkingTheCloseEquitiesParameters(
                "0",
                new TimeSpan(parameters.WindowHours, 0, 0),
                parameters.PercentageThresholdDailyVolume,
                parameters.PercentageThresholdWindowVolume,
                null,
                new[] { ClientOrganisationalFactors.None },
                true,
                true);
        }

        [Then(@"I will have (.*) marking the close alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }

        [When(@"I run the marking the close rule")]
        public void WhenIRunTheMarkingTheCloseRule()
        {
            var cancelledOrders = this._equityRuleMarkingTheCloseFactory.Build(
                this._equitiesParameters,
                this._ruleCtx,
                this._alertStream,
                RuleRunMode.ForceRun,
                this._dataRequestSubscriber);

            foreach (var universeEvent in this._universeSelectionState.SelectedUniverse.UniverseEvents)
                cancelledOrders.OnNext(universeEvent);
        }
    }
}
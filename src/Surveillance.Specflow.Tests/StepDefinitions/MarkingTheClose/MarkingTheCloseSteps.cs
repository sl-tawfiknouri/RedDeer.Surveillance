using System;
using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
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
using Surveillance.Specflow.Tests.StepDefinitions.Universe;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions.MarkingTheClose
{
    [Binding]
    public sealed class MarkingTheCloseSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly UniverseSelectionState _universeSelectionState;

        private readonly IUniverseAlertStream _alertStream;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IEquityRuleMarkingTheCloseFactory _equityRuleMarkingTheCloseFactory;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private IUniverseEquityOrderFilterService _universeOrderFilterService;
        private UniverseMarketCacheFactory _universeMarketCacheFactory;
        private MarkingTheCloseEquitiesParameters _equitiesParameters;

        public MarkingTheCloseSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState)
        {
            _scenarioContext = scenarioContext;
            _universeSelectionState = universeSelectionState;

            _universeMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            _alertStream = A.Fake<IUniverseAlertStream>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _universeOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();

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

            _tradingHoursService = new MarketTradingHoursService(repository, new NullLogger<MarketTradingHoursService>());
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            _equityRuleMarkingTheCloseFactory = new EquityRuleMarkingTheCloseFactory(
                _universeOrderFilterService,
                _universeMarketCacheFactory,
                _tradingHoursService,
                new NullLogger<MarkingTheCloseRule>(),
                new NullLogger<TradingHistoryStack>());
        }

        [Given(@"I have the marking the close rule parameter values")]
        public void GivenIHaveTheMarkingTheCloseRuleParameterValues(Table markingTheCloseParameters)
        {
            if (markingTheCloseParameters.RowCount != 1)
            {
                _scenarioContext.Pending();
                return;
            }

            var parameters = markingTheCloseParameters.CreateInstance<MarkingTheCloseApiParameters>();

            _equitiesParameters = new MarkingTheCloseEquitiesParameters(
                "0",
                new System.TimeSpan(parameters.WindowHours, 0, 0),
                parameters.PercentageThresholdDailyVolume,
                parameters.PercentageThresholdWindowVolume,
                null,
                new[] { ClientOrganisationalFactors.None },
                true);
        }

        [When(@"I run the marking the close rule")]
        public void WhenIRunTheMarkingTheCloseRule()
        {
            var cancelledOrders =
                _equityRuleMarkingTheCloseFactory.Build(
                    _equitiesParameters,
                    _ruleCtx,
                    _alertStream,
                    RuleRunMode.ForceRun,
                    _dataRequestSubscriber);

            foreach (var universeEvent in _universeSelectionState.SelectedUniverse.UniverseEvents)
                cancelledOrders.OnNext(universeEvent);
        }

        [Then(@"I will have (.*) marking the close alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }

    }
}

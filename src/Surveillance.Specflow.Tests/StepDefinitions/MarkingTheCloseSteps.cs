using System;
using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.MarkingTheClose;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.MarkingTheClose;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions
{
    [Binding]
    public sealed class MarkingTheCloseSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly UniverseSelectionState _universeSelectionState;

        private readonly IUniverseAlertStream _alertStream;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IMarkingTheCloseRuleFactory _markingTheCloseFactory;
        private readonly IMarketTradingHoursManager _tradingHoursManager;
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private IUniverseOrderFilter _universeOrderFilter;
        private UniverseMarketCacheFactory _universeMarketCacheFactory;
        private MarkingTheCloseParameters _parameters;

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
            _universeOrderFilter = A.Fake<IUniverseOrderFilter>();
            _tradingHoursManager = A.Fake<IMarketTradingHoursManager>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            A
                .CallTo(() => _tradingHoursManager.GetTradingHoursForMic("XLON"))
                .Returns(new TradingHours
                {
                    IsValid = true,
                    Mic = "XLON",
                    OpenOffsetInUtc = TimeSpan.FromHours(8),
                    CloseOffsetInUtc = TimeSpan.FromHours(16)
                });

            A
                .CallTo(() => _tradingHoursManager.GetTradingHoursForMic("NASDAQ"))
                .Returns(new TradingHours
                {
                    IsValid = true,
                    Mic = "NASDAQ",
                    OpenOffsetInUtc = TimeSpan.FromHours(16),
                    CloseOffsetInUtc = TimeSpan.FromHours(23)
                });

            _markingTheCloseFactory = new MarkingTheCloseRuleFactory(
                _universeOrderFilter,
                _universeMarketCacheFactory,
                _tradingHoursManager,
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

            _parameters = new MarkingTheCloseParameters(
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
                _markingTheCloseFactory.Build(
                    _parameters,
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

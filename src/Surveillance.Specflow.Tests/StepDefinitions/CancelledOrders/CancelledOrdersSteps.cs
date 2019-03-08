using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.Universe;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions.CancelledOrders
{
    [Binding]
    public class CancelledOrdersSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly UniverseSelectionState _universeSelectionState;

        private readonly IUniverseAlertStream _alertStream;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IEquityRuleCancelledOrderFactory _equityRuleCancelledOrderFactory;

        private IUniverseEquityOrderFilterService _universeOrderFilterService;
        private UniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;

        private CancelledOrderRuleEquitiesParameters _parameters;

        public CancelledOrdersSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState)
        {
            _scenarioContext = scenarioContext;
            _universeSelectionState = universeSelectionState;

            _interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            _alertStream = A.Fake<IUniverseAlertStream>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _universeOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();

            _equityRuleCancelledOrderFactory = new EquityRuleCancelledOrderFactory(
                _universeOrderFilterService,
                _interdayUniverseMarketCacheFactory,
                new NullLogger<CancelledOrderRule>(),
                new NullLogger<TradingHistoryStack>());
        }

        [Given(@"I have the cancelled orders rule parameter values")]
        public void GivenIHaveTheCancelledOrderRuleParameterValues(Table cancelledOrderParameters)
        {
            if (cancelledOrderParameters.RowCount != 1)
            {
                _scenarioContext.Pending();
                return;
            }

            var parameters = cancelledOrderParameters.CreateInstance<CancelledOrderApiParameters>();

            _parameters = new CancelledOrderRuleEquitiesParameters(
                "0",
                new System.TimeSpan(parameters.WindowHours, 0, 0),
                parameters.CancelledOrderPercentagePositionThreshold,
                parameters.CancelledOrderCountPercentageThreshold,
                parameters.MinimumNumberOfTradesToApplyRuleTo,
                parameters.MaximumNumberOfTradesToApplyRuleTo,
                new []{ ClientOrganisationalFactors.None},
                true);
        }

        [When(@"I run the cancelled orders rule")]
        public void WhenIRunTheCancelledOrderRule()
        {
            var cancelledOrders =
                _equityRuleCancelledOrderFactory.Build(
                    _parameters,
                    _ruleCtx,
                    _alertStream,
                    RuleRunMode.ForceRun);

            foreach (var universeEvent in _universeSelectionState.SelectedUniverse.UniverseEvents)
                cancelledOrders.OnNext(universeEvent);
        }

        [Then(@"I will have (.*) cancelled orders alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }
    }
}

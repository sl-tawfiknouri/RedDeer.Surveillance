namespace Surveillance.Specflow.Tests.StepDefinitions.CancelledOrders
{
    using System;

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

    [Binding]
    public class CancelledOrdersSteps
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly IEquityRuleCancelledOrderFactory _equityRuleCancelledOrderFactory;

        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        private readonly ScenarioContext _scenarioContext;

        private readonly UniverseSelectionState _universeSelectionState;

        private readonly UniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;

        private CancelledOrderRuleEquitiesParameters _parameters;

        private readonly IUniverseEquityOrderFilterService _universeOrderFilterService;

        public CancelledOrdersSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
        {
            this._scenarioContext = scenarioContext;
            this._universeSelectionState = universeSelectionState;

            this._interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            this._alertStream = A.Fake<IUniverseAlertStream>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._universeOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();

            this._equityRuleCancelledOrderFactory = new EquityRuleCancelledOrderFactory(
                this._universeOrderFilterService,
                this._interdayUniverseMarketCacheFactory,
                new NullLogger<CancelledOrderRule>(),
                new NullLogger<TradingHistoryStack>());
        }

        [Given(@"I have the cancelled orders rule parameter values")]
        public void GivenIHaveTheCancelledOrderRuleParameterValues(Table cancelledOrderParameters)
        {
            if (cancelledOrderParameters.RowCount != 1)
            {
                this._scenarioContext.Pending();
                return;
            }

            var parameters = cancelledOrderParameters.CreateInstance<CancelledOrderApiParameters>();

            this._parameters = new CancelledOrderRuleEquitiesParameters(
                "0",
                new TimeSpan(parameters.WindowHours, 0, 0),
                parameters.CancelledOrderPercentagePositionThreshold,
                parameters.CancelledOrderCountPercentageThreshold,
                parameters.MinimumNumberOfTradesToApplyRuleTo,
                parameters.MaximumNumberOfTradesToApplyRuleTo,
                new[] { ClientOrganisationalFactors.None },
                true,
                true);
        }

        [Then(@"I will have (.*) cancelled orders alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }

        [When(@"I run the cancelled orders rule")]
        public void WhenIRunTheCancelledOrderRule()
        {
            var cancelledOrders = this._equityRuleCancelledOrderFactory.Build(
                this._parameters,
                this._ruleCtx,
                this._alertStream,
                RuleRunMode.ForceRun);

            foreach (var universeEvent in this._universeSelectionState.SelectedUniverse.UniverseEvents)
                cancelledOrders.OnNext(universeEvent);
        }
    }
}
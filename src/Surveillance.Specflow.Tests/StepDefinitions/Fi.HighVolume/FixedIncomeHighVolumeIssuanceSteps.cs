using System;
using Domain.Surveillance.Scheduling;
using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.Universe;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions.Fi.HighVolume
{
    [Binding]
    public class FixedIncomeHighVolumeIssuanceSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private UniverseSelectionState _universeSelectionState;
        private HighVolumeIssuanceRuleFixedIncomeParameters _parameters;

        private FixedIncomeHighProfitsRule _rule;

        private IUniverseFixedIncomeOrderFilterService _orderFilterService;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        private UniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;

        public FixedIncomeHighVolumeIssuanceSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState)
        {
            _scenarioContext = scenarioContext;
            _universeSelectionState = universeSelectionState;
        }

        private void Setup()
        {
            _orderFilterService = A.Fake<IUniverseFixedIncomeOrderFilterService>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();

            _interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());
        }

        [Given(@"I have the fixed income high volume issuance rule parameter values")]
        public void GivenIHaveTheFixedIncomeHighVolumeIssuanceRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                _scenarioContext.Pending();
                return;
            }

            var parameters = ruleParameters.CreateInstance<FixedIncomeHighVolumeIssuanceApiParameters>();

            _parameters = new HighVolumeIssuanceRuleFixedIncomeParameters(
                "0",
                TimeSpan.FromHours(parameters.WindowHours),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                new[] { ClientOrganisationalFactors.None },
                true,
                true);
        }

        [When(@"I run the fixed income high volume issuance rule")]
        public void WhenIRunTheFixedIncomeHighProfitRule()
        {
            var scheduledExecution = new ScheduledExecution { IsForceRerun = true };

            Setup();

            var rule = new FixedIncomeHighVolumeIssuanceRule(
                _parameters,
                _orderFilterService,
                _ruleCtx,
                _interdayUniverseMarketCacheFactory,
                RuleRunMode.ForceRun,
                _alertStream,
                new NullLogger<FixedIncomeHighVolumeIssuanceRule>(),
                new NullLogger<TradingHistoryStack>());

            foreach (var universeEvent in _universeSelectionState.SelectedUniverse.UniverseEvents)
                rule.OnNext(universeEvent);
        }

        [Then(@"I will have (.*) fixed income high volume issuance alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A
                .CallTo(() =>
                    _alertStream.Add(A<IUniverseAlertEvent>.That.Matches(i => !i.IsDeleteEvent && !i.IsRemoveEvent)))
                .MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }

    }
}

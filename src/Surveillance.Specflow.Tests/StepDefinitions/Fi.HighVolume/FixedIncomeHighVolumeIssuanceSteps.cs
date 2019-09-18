namespace Surveillance.Specflow.Tests.StepDefinitions.Fi.HighVolume
{
    using System;

    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using Microsoft.Extensions.Logging.Abstractions;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora.BMLL;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    /// <summary>
    /// The fixed income high volume issuance steps.
    /// </summary>
    [Binding]
    public class FixedIncomeHighVolumeIssuanceSteps
    {
        /// <summary>
        /// The scenario context.
        /// </summary>
        private readonly ScenarioContext scenarioContext;

        /// <summary>
        /// The universe selection state.
        /// </summary>
        private readonly UniverseSelectionState universeSelectionState;

        /// <summary>
        /// The alert stream.
        /// </summary>
        private IUniverseAlertStream alertStream;

        /// <summary>
        /// The interday universe market cache factory.
        /// </summary>
        private UniverseMarketCacheFactory interdayUniverseMarketCacheFactory;

        /// <summary>
        /// The order filter service.
        /// </summary>
        private IUniverseFixedIncomeOrderFilterService orderFilterService;

        /// <summary>
        /// The parameters.
        /// </summary>
        private HighVolumeIssuanceRuleFixedIncomeParameters parameters;

        /// <summary>
        /// The rule context.
        /// </summary>
        private ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// The judgement service.
        /// </summary>
        private IFixedIncomeHighVolumeJudgementService judgementService;

        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighVolumeIssuanceSteps"/> class.
        /// </summary>
        /// <param name="scenarioContext">
        /// The scenario context.
        /// </param>
        /// <param name="universeSelectionState">
        /// The universe selection state.
        /// </param>
        public FixedIncomeHighVolumeIssuanceSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState)
        {
            this.scenarioContext = scenarioContext;
            this.universeSelectionState = universeSelectionState;
        }

        /// <summary>
        /// The given i have the fixed income high volume issuance rule parameter values.
        /// </summary>
        /// <param name="ruleParameters">
        /// The rule parameters.
        /// </param>
        [Given(@"I have the fixed income high volume issuance rule parameter values")]
        public void GivenIHaveTheFixedIncomeHighVolumeIssuanceRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                this.scenarioContext.Pending();
                return;
            }

            var highVolumeParameters = ruleParameters.CreateInstance<FixedIncomeHighVolumeIssuanceApiParameters>();

            this.parameters = new HighVolumeIssuanceRuleFixedIncomeParameters(
                "0",
                TimeSpan.FromHours(highVolumeParameters.WindowHours),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                new[] { ClientOrganisationalFactors.None },
                true,
                true);
        }

        /// <summary>
        /// The then i will have alerts.
        /// </summary>
        /// <param name="alertCount">
        /// The alert count.
        /// </param>
        [Then(@"I will have (.*) fixed income high volume issuance alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(
                    () => this.alertStream.Add(
                        A<IUniverseAlertEvent>.That.Matches(i => !i.IsDeleteEvent && !i.IsRemoveEvent)))
                .MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }

        /// <summary>
        /// The when i run the fixed income high profit rule.
        /// </summary>
        [When(@"I run the fixed income high volume issuance rule")]
        public void WhenIRunTheFixedIncomeHighProfitRule()
        {
            var scheduledExecution = new ScheduledExecution { IsForceRerun = true };

            this.Setup();

            var rule = new FixedIncomeHighVolumeRule(
                this.parameters,
                this.orderFilterService,
                this.ruleContext,
                this.interdayUniverseMarketCacheFactory,
                this.judgementService,
                this.dataRequestSubscriber,
                RuleRunMode.ForceRun,
                new NullLogger<FixedIncomeHighVolumeRule>(),
                new NullLogger<TradingHistoryStack>());

            foreach (var universeEvent in this.universeSelectionState.SelectedUniverse.UniverseEvents)
            {
                rule.OnNext(universeEvent);
            }
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        private void Setup()
        {
            this.orderFilterService = A.Fake<IUniverseFixedIncomeOrderFilterService>();
            this.ruleContext = A.Fake<ISystemProcessOperationRunRuleContext>();
            this.alertStream = A.Fake<IUniverseAlertStream>();
            this.judgementService = A.Fake<IJudgementService>();
            this.dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            this.interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());
        }
    }
}
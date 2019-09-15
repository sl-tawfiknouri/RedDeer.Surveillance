namespace Surveillance.Specflow.Tests.StepDefinitions.Fi.HighProfit
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging.Abstractions;

    using RedDeer.Contracts.SurveillanceService.Rules;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora.BMLL;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    /// <summary>
    /// The fixed income high profit steps.
    /// </summary>
    [Binding]
    public class FixedIncomeHighProfitSteps
    {
        /// <summary>
        /// The _scenario context.
        /// </summary>
        private readonly ScenarioContext scenarioContext;

        /// <summary>
        /// The _universe selection state.
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
        private HighProfitsRuleFixedIncomeParameters parameters;

        /// <summary>
        /// The rule context for the system operation.
        /// </summary>
        private ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighProfitSteps"/> class.
        /// </summary>
        /// <param name="scenarioContext">
        /// The scenario context.
        /// </param>
        /// <param name="universeSelectionState">
        /// The universe selection state.
        /// </param>
        public FixedIncomeHighProfitSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState)
        {
            this.scenarioContext = scenarioContext;
            this.universeSelectionState = universeSelectionState;
        }

        /// <summary>
        /// The given i have the fixed income high profit rule parameter values.
        /// </summary>
        /// <param name="ruleParameters">
        /// The rule parameters.
        /// </param>
        [Given(@"I have the fixed income high profit rule parameter values")]
        public void GivenIHaveTheFixedIncomeHighProfitRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                this.scenarioContext.Pending();
                return;
            }

            var parameters = ruleParameters.CreateInstance<FixedIncomeHighProfitApiParameters>();

            this.parameters = new HighProfitsRuleFixedIncomeParameters(
                "0",
                TimeSpan.FromHours(parameters.WindowHours),
                TimeSpan.FromHours(parameters.FutureHours),
                parameters.PerformHighProfitWindowAnalysis,
                parameters.PerformHighProfitDailyAnalysis,
                parameters.HighProfitPercentage,
                parameters.HighProfitAbsolute,
                parameters.HighProfitUseCurrencyConversions,
                parameters.HighProfitCurrency,
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
        [Then(@"I will have (.*) fixed income high profit alerts")]
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
        [When(@"I run the fixed income high profit rule")]
        public void WhenIRunTheFixedIncomeHighProfitRule()
        {
            var scheduledExecution = new ScheduledExecution { IsForceRerun = true };

            this.Setup();

            var rule = new FixedIncomeHighProfitsRule(
                this.parameters,
                null,
                null,
                new NullLogger<FixedIncomeHighProfitsRule>());

            foreach (var universeEvent in this.universeSelectionState.SelectedUniverse.UniverseEvents)
                rule.OnNext(universeEvent);
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        private void Setup()
        {
            this.orderFilterService = A.Fake<IUniverseFixedIncomeOrderFilterService>();
            this.ruleContext = A.Fake<ISystemProcessOperationRunRuleContext>();
            this.alertStream = A.Fake<IUniverseAlertStream>();

            this.interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());
        }
    }
}
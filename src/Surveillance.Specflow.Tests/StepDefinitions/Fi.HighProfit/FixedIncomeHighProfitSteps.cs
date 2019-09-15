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
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public class FixedIncomeHighProfitSteps
    {
        private readonly ScenarioContext _scenarioContext;

        private readonly UniverseSelectionState _universeSelectionState;

        private IUniverseAlertStream _alertStream;

        private UniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;

        private IUniverseFixedIncomeOrderFilterService _orderFilterService;

        private HighProfitsRuleFixedIncomeParameters _parameters;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        public FixedIncomeHighProfitSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState)
        {
            this._scenarioContext = scenarioContext;
            this._universeSelectionState = universeSelectionState;
        }

        [Given(@"I have the fixed income high profit rule parameter values")]
        public void GivenIHaveTheFixedIncomeHighProfitRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                this._scenarioContext.Pending();
                return;
            }

            var parameters = ruleParameters.CreateInstance<FixedIncomeHighProfitApiParameters>();

            this._parameters = new HighProfitsRuleFixedIncomeParameters(
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

        [Then(@"I will have (.*) fixed income high profit alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(
                    () => this._alertStream.Add(
                        A<IUniverseAlertEvent>.That.Matches(i => !i.IsDeleteEvent && !i.IsRemoveEvent)))
                .MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }

        [When(@"I run the fixed income high profit rule")]
        public void WhenIRunTheFixedIncomeHighProfitRule()
        {
            var scheduledExecution = new ScheduledExecution { IsForceRerun = true };

            this.Setup();

            var rule = new FixedIncomeHighProfitsRule(
                this._parameters,
                null,
                new NullLogger<FixedIncomeHighProfitsRule>());

            foreach (var universeEvent in this._universeSelectionState.SelectedUniverse.UniverseEvents)
                rule.OnNext(universeEvent);
        }

        private void Setup()
        {
            this._orderFilterService = A.Fake<IUniverseFixedIncomeOrderFilterService>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();

            this._interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());
        }
    }
}
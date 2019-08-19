namespace Surveillance.Specflow.Tests.StepDefinitions.Fi.WashTrade
{
    using System;

    using Domain.Core.Trading.Factories;
    using Domain.Core.Trading.Factories.Interfaces;
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
    using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public class FixedIncomeWashTradeSteps
    {
        private readonly ScenarioContext _scenarioContext;

        private readonly UniverseSelectionState _universeSelectionState;

        private IUniverseAlertStream _alertStream;

        private IClusteringService _clusteringService;

        private UniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;

        private IUniverseFixedIncomeOrderFilterService _orderFilterService;

        private WashTradeRuleFixedIncomeParameters _parameters;

        private IPortfolioFactory _portfolioFactory;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        public FixedIncomeWashTradeSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
        {
            this._scenarioContext = scenarioContext;
            this._universeSelectionState = universeSelectionState;
        }

        [Given(@"I have the fixed income wash trade rule parameter values")]
        public void GivenIHaveTheFixedIncomeWashTradeRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                this._scenarioContext.Pending();
                return;
            }

            var parameters = ruleParameters.CreateInstance<FixedIncomeWashTradeApiParameters>();

            this._parameters = new WashTradeRuleFixedIncomeParameters(
                "0",
                TimeSpan.FromHours(parameters.WindowHours),
                parameters.PerformAveragePositionAnalysis,
                parameters.PerformClusteringPositionAnalysis,
                parameters.AveragePositionMinimumNumberOfTrades,
                parameters.AveragePositionMaximumPositionValueChange,
                parameters.AveragePositionMaximumAbsoluteValueChangeAmount,
                parameters.AveragePositionMaximumAbsoluteValueChangeCurrency,
                parameters.ClusteringPositionMinimumNumberOfTrades,
                parameters.ClusteringPercentageValueDifferenceThreshold,
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                new[] { ClientOrganisationalFactors.None },
                true,
                true);
        }

        [Then(@"I will have (.*) fixed income wash trade alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(
                    () => this._alertStream.Add(
                        A<IUniverseAlertEvent>.That.Matches(i => !i.IsDeleteEvent && !i.IsRemoveEvent)))
                .MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }

        [When(@"I run the fixed income wash trade rule")]
        public void WhenIRunTheFixedIncomeHighProfitRule()
        {
            var scheduledExecution = new ScheduledExecution { IsForceRerun = true };

            this.Setup();

            var rule = new FixedIncomeWashTradeRule(
                this._parameters,
                this._orderFilterService,
                this._ruleCtx,
                this._interdayUniverseMarketCacheFactory,
                RuleRunMode.ForceRun,
                this._alertStream,
                this._clusteringService,
                this._portfolioFactory,
                new NullLogger<FixedIncomeWashTradeRule>(),
                new NullLogger<TradingHistoryStack>());

            foreach (var universeEvent in this._universeSelectionState.SelectedUniverse.UniverseEvents)
                rule.OnNext(universeEvent);
        }

        private void Setup()
        {
            this._orderFilterService = A.Fake<IUniverseFixedIncomeOrderFilterService>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();

            this._portfolioFactory = new PortfolioFactory();
            this._clusteringService = new ClusteringService();
            this._interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());
        }
    }
}
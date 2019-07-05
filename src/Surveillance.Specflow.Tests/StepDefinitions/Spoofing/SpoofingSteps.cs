using System;
using Domain.Core.Trading.Execution;
using Domain.Core.Trading.Execution.Interfaces;
using Domain.Core.Trading.Factories;
using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.Spoofing;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.Universe;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions.Spoofing
{
    [Binding]
    public class SpoofingSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly UniverseSelectionState _universeSelectionState;
        private SpoofingRuleEquitiesParameters _parameters;
        
        private readonly IUniverseAlertStream _alertStream;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IEquityRuleSpoofingFactory _equityRuleSpoofingFactory;
        private readonly IOrderAnalysisService _orderAnalysisService;

        public SpoofingSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState)
        {
            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            _universeSelectionState = universeSelectionState ?? throw new ArgumentNullException(nameof(universeSelectionState));
            _orderAnalysisService = new OrderAnalysisService();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();

            var universeMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            _equityRuleSpoofingFactory =
                new EquityRuleSpoofingFactory(
                    universeMarketCacheFactory,
                    new UniverseEquityOrderFilterService(new NullLogger<UniverseEquityOrderFilterService>()),
                    new PortfolioFactory(),
                    _orderAnalysisService,
                    new NullLogger<SpoofingRule>(),
                    new NullLogger<TradingHistoryStack>());
        }

        [Given(@"I have the spoofing rule parameter values")]
        public void GivenIHaveTheSpoofingRuleParameterValues(Table spoofingParameters)
        {
            if (spoofingParameters.RowCount != 1)
            {
                _scenarioContext.Pending();
                return;
            }

            var parameters = spoofingParameters.CreateInstance<SpoofingApiParameters>();

            _parameters =
                new SpoofingRuleEquitiesParameters(
                    "0",
                    new TimeSpan(parameters.WindowHours, 0, 0),
                    parameters.CancellationThreshold,
                    parameters.RelativeSizeMultipleForSpoofExceedingReal,
                    new ClientOrganisationalFactors[0],
                    true,
                    true);
        }

        [When(@"I run the spoofing rule")]
        public void WhenIRunSpoofingRule()
        {
            var spoofingRule =
                _equityRuleSpoofingFactory
                    .Build(
                        _parameters,
                        _ruleCtx,
                        _alertStream,
                        RuleRunMode.ForceRun);

            foreach (var universeEvent in _universeSelectionState.SelectedUniverse.UniverseEvents)
                spoofingRule.OnNext(universeEvent);
        }

        [Then(@"I will have (.*) spoofing alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }
    }
}

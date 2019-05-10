using System;
using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Surveillance.Auditing.Context.Interfaces;
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
using Surveillance.Engine.Rules.Rules.Equity.Ramping;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.Universe;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions.Ramping
{
    [Binding]
    public class RampingSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly UniverseSelectionState _universeSelectionState;
        private RampingRuleEquitiesParameters _parameters;

        private readonly IRampingAnalyser _rampingAnalyser;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly IUniverseEquityOrderFilterService _equityOrderFilterService;

        private readonly IUniverseAlertStream _alertStream;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IEquityRuleRampingFactory _equityRuleRampingFactory;
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        public RampingSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState)
        {
            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            _universeSelectionState = universeSelectionState ?? throw new ArgumentNullException(nameof(universeSelectionState));
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _tradingHoursService = A.Fake<IMarketTradingHoursService>();
            _rampingAnalyser =
                new RampingAnalyser(
                    new TimeSeriesTrendClassifier(
                        new NullLogger<TimeSeriesTrendClassifier>()),
                    new OrderPriceImpactClassifier());
            _equityOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();

            A
                .CallTo(() => _tradingHoursService.GetTradingHoursForMic("XLON"))
                .Returns(new TradingHours
                {
                    IsValid = true,
                    Mic = "XLON",
                    OpenOffsetInUtc = TimeSpan.FromHours(8),
                    CloseOffsetInUtc = TimeSpan.FromHours(16)
                });

            A
                .CallTo(() => _tradingHoursService.GetTradingHoursForMic("NASDAQ"))
                .Returns(new TradingHours
                {
                    IsValid = true,
                    Mic = "NASDAQ",
                    OpenOffsetInUtc = TimeSpan.FromHours(16),
                    CloseOffsetInUtc = TimeSpan.FromHours(23)
                });

            var universeMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            _equityRuleRampingFactory =
                new EquityRuleRampingFactory(
                    _rampingAnalyser,
                    _equityOrderFilterService,
                    universeMarketCacheFactory,
                    _dataRequestSubscriber,
                    _tradingHoursService,
                    new NullLogger<RampingRule>(),
                    new NullLogger<TradingHistoryStack>());
        }

        [Given(@"I have the ramping rule parameter values")]
        public void GivenIHaveTheRampingRuleParameterValues(Table rampingParameters)
        {
            if (rampingParameters.RowCount != 1)
            {
                _scenarioContext.Pending();
                return;
            }

            var parameters = rampingParameters.CreateInstance<RampingApiParameters>();

            _parameters =
                new RampingRuleEquitiesParameters(
                    "0",
                    new TimeSpan(parameters.WindowHours, 0, 0),
                    parameters.AutoCorrelationCoefficient,
                    parameters.ThresholdOrdersExecutedInWindow,
                    parameters.ThresholdVolumePercentageWindow,
                    new ClientOrganisationalFactors[0],
                    true);
        }

        [When(@"I run the ramping rule")]
        public void WhenIRunRampingRule()
        {
            var rampingRule =
                _equityRuleRampingFactory
                    .Build(
                        _parameters,
                        _ruleCtx,
                        _alertStream,
                        RuleRunMode.ForceRun,
                        _dataRequestSubscriber);

            foreach (var universeEvent in _universeSelectionState.SelectedUniverse.UniverseEvents)
                rampingRule.OnNext(universeEvent);
        }

        [Then(@"I will have (.*) ramping alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }
    }
}

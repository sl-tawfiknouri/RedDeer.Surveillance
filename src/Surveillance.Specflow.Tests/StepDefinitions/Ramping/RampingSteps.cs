namespace Surveillance.Specflow.Tests.StepDefinitions.Ramping
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging.Abstractions;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

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
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public class RampingSteps
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private readonly IUniverseEquityOrderFilterService _equityOrderFilterService;

        private readonly IEquityRuleRampingFactory _equityRuleRampingFactory;

        private readonly IRampingAnalyser _rampingAnalyser;

        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        private readonly ScenarioContext _scenarioContext;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private readonly UniverseSelectionState _universeSelectionState;

        private RampingRuleEquitiesParameters _parameters;

        public RampingSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
        {
            this._scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            this._universeSelectionState =
                universeSelectionState ?? throw new ArgumentNullException(nameof(universeSelectionState));
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            this._alertStream = A.Fake<IUniverseAlertStream>();

            var repository = A.Fake<IMarketOpenCloseApiCachingDecorator>();

            A.CallTo(() => repository.GetAsync()).Returns(
                new[]
                    {
                        new ExchangeDto
                            {
                                Code = "XLON",
                                MarketOpenTime = TimeSpan.FromHours(8),
                                MarketCloseTime = TimeSpan.FromHours(16),
                                IsOpenOnMonday = true,
                                IsOpenOnTuesday = true,
                                IsOpenOnWednesday = true,
                                IsOpenOnThursday = true,
                                IsOpenOnFriday = true,
                                IsOpenOnSaturday = true,
                                IsOpenOnSunday = true
                            },
                        new ExchangeDto
                            {
                                Code = "NASDAQ",
                                MarketOpenTime = TimeSpan.FromHours(15),
                                MarketCloseTime = TimeSpan.FromHours(23),
                                IsOpenOnMonday = true,
                                IsOpenOnTuesday = true,
                                IsOpenOnWednesday = true,
                                IsOpenOnThursday = true,
                                IsOpenOnFriday = true,
                                IsOpenOnSaturday = true,
                                IsOpenOnSunday = true
                            }
                    });

            this._tradingHoursService = new MarketTradingHoursService(
                repository,
                new NullLogger<MarketTradingHoursService>());
            this._rampingAnalyser = new RampingAnalyser(
                new TimeSeriesTrendClassifier(new NullLogger<TimeSeriesTrendClassifier>()),
                new OrderPriceImpactClassifier());
            this._equityOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();

            var equityMarketCacheFactory = new UniverseEquityMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseEquityMarketCacheFactory>());

            var fixedIncomeMarketCacheFactory = new UniverseFixedIncomeMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseFixedIncomeMarketCacheFactory>());

            this._equityRuleRampingFactory = new EquityRuleRampingFactory(
                this._rampingAnalyser,
                this._equityOrderFilterService,
                equityMarketCacheFactory,
                fixedIncomeMarketCacheFactory,
                this._tradingHoursService,
                new NullLogger<RampingRule>(),
                new NullLogger<TradingHistoryStack>());
        }

        [Given(@"I have the ramping rule parameter values")]
        public void GivenIHaveTheRampingRuleParameterValues(Table rampingParameters)
        {
            if (rampingParameters.RowCount != 1)
            {
                this._scenarioContext.Pending();
                return;
            }

            var parameters = rampingParameters.CreateInstance<RampingApiParameters>();

            this._parameters = new RampingRuleEquitiesParameters(
                "0",
                new TimeSpan(parameters.WindowHours, 0, 0),
                parameters.AutoCorrelationCoefficient,
                parameters.ThresholdOrdersExecutedInWindow,
                parameters.ThresholdVolumePercentageWindow,
                new ClientOrganisationalFactors[0],
                true,
                true);
        }

        [Then(@"I will have (.*) ramping alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }

        [When(@"I run the ramping rule")]
        public void WhenIRunRampingRule()
        {
            var rampingRule = this._equityRuleRampingFactory.Build(
                this._parameters,
                this._ruleCtx,
                this._alertStream,
                RuleRunMode.ForceRun,
                this._dataRequestSubscriber);

            foreach (var universeEvent in this._universeSelectionState.SelectedUniverse.UniverseEvents)
                rampingRule.OnNext(universeEvent);
        }
    }
}
namespace Surveillance.Specflow.Tests.StepDefinitions.Spoofing
{
    using System;

    using Domain.Core.Trading.Execution;
    using Domain.Core.Trading.Execution.Interfaces;
    using Domain.Core.Trading.Factories;

    using FakeItEasy;

    using Microsoft.Extensions.Logging.Abstractions;
    using RedDeer.Contracts.SurveillanceService.Api.Markets;
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora.BMLL;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.Layering;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public class LayeringSteps
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly IEquityRuleLayeringFactory _equityRuleLayeringFactory;

        private readonly IOrderAnalysisService _orderAnalysisService;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        private readonly ScenarioContext _scenarioContext;

        private readonly UniverseSelectionState _universeSelectionState;

        private LayeringRuleEquitiesParameters _parameters;

        public LayeringSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
        {
            this._scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            this._universeSelectionState =
                universeSelectionState ?? throw new ArgumentNullException(nameof(universeSelectionState));
            this._orderAnalysisService = new OrderAnalysisService();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();

            var equityMarketCacheFactory = new UniverseEquityMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseEquityMarketCacheFactory>());

            var fixedIncomeMarketCacheFactory = new UniverseFixedIncomeMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseFixedIncomeMarketCacheFactory>());

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

            this._equityRuleLayeringFactory = new EquityRuleLayeringFactory(
                new UniverseEquityOrderFilterService(new NullLogger<UniverseEquityOrderFilterService>()),
                _tradingHoursService,
                equityMarketCacheFactory,
                fixedIncomeMarketCacheFactory,
                new NullLogger<EquityRuleLayeringFactory>(),
                new NullLogger<TradingHistoryStack>());
        }

        [Given(@"I have the layering rule parameter values")]
        public void GivenIHaveTheLayeringRuleParameterValues(Table layeringParameters)
        {
            if (layeringParameters.RowCount != 1)
            {
                this._scenarioContext.Pending();
                return;
            }

            var parameters = layeringParameters.CreateInstance<LayeringApiParameters>();

            this._parameters = new LayeringRuleEquitiesParameters(
                "0",
                new TimeSpan(parameters.WindowHours, 0, 0),
                parameters.PercentageOfMarketDailyVolume,
                parameters.PercentageOfMarketWindowVolume,
                parameters.CheckForCorrespondingPriceMovement,
                new ClientOrganisationalFactors[0],
                true,
                true);
        }

        [Then(@"I will have (.*) layering alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }

        [When(@"I run the layering rule")]
        public void WhenIRunLayeringRule()
        {
            var layeringRule = this._equityRuleLayeringFactory.Build(
                this._parameters,
                this._ruleCtx,
                this._alertStream,
                RuleRunMode.ForceRun);

            foreach (var universeEvent in this._universeSelectionState.SelectedUniverse.UniverseEvents)
            {
                layeringRule.OnNext(universeEvent);
            }
        }
    }
}
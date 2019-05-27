using System;
using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute;
using Surveillance.Engine.Rules.Rules.Equity.Ramping;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.Universe;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions.PlacingOrders
{
    [Binding]
    public class PlacingOrdersSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly UniverseSelectionState _universeSelectionState;
        private PlacingOrderWithNoIntentToExecuteRuleEquitiesParameters _parameters;

        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly IUniverseEquityOrderFilterService _equityOrderFilterService;

        private readonly IUniverseAlertStream _alertStream;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IEquityRulePlacingOrdersWithoutIntentToExecuteFactory _equityRulePlacingOrdersFactory;
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        public PlacingOrdersSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState)
        {
            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            _universeSelectionState = universeSelectionState ?? throw new ArgumentNullException(nameof(universeSelectionState));

            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _equityOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();

            var repository = A.Fake<IMarketOpenCloseApiCachingDecoratorRepository>();

            A
                .CallTo(() => repository.Get()).
                Returns(new ExchangeDto[]
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
                        IsOpenOnSunday = true,
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
                        IsOpenOnSunday = true,
                    }
                });

            _tradingHoursService = new MarketTradingHoursService(repository, new NullLogger<MarketTradingHoursService>());

            var universeMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            _equityRulePlacingOrdersFactory =
                new EquityRulePlacingOrdersWithoutIntentToExecuteFactory(
                    _equityOrderFilterService,
                    universeMarketCacheFactory,
                    _tradingHoursService,
                    new NullLogger<PlacingOrdersWithNoIntentToExecuteRule>(),
                    new NullLogger<TradingHistoryStack>());
        }

        [Given(@"I have the placing orders rule parameter values")]
        public void GivenIHaveThePlacingOrdersRuleParameterValues(Table placingOrdersParameters)
        {
            if (placingOrdersParameters.RowCount != 1)
            {
                _scenarioContext.Pending();
                return;
            }

            var parameters = placingOrdersParameters.CreateInstance<PlacingOrdersApiParameters>();

            _parameters =
                new PlacingOrderWithNoIntentToExecuteRuleEquitiesParameters(
                    "0",
                    parameters.Sigma,
                    new TimeSpan(parameters.WindowHours, 0, 0),
                    new ClientOrganisationalFactors[0],
                    true);
        }

        [When(@"I run the placing orders rule")]
        public void WhenIRunPlacingOrdersRule()
        {
            var placingOrdersRule =
                _equityRulePlacingOrdersFactory
                    .Build(
                        _parameters,
                        _alertStream,
                        _ruleCtx,
                        _dataRequestSubscriber,
                        RuleRunMode.ForceRun);

            foreach (var universeEvent in _universeSelectionState.SelectedUniverse.UniverseEvents)
                placingOrdersRule.OnNext(universeEvent);
        }

        [Then(@"I will have (.*) placing orders alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }
    }
}

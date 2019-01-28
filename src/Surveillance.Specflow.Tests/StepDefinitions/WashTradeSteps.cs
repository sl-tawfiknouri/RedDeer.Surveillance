using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Currency.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters;
using Surveillance.RuleParameters.OrganisationalFactors;
using Surveillance.Rules.WashTrade;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.WashTrades;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Universe.Filter.Interfaces;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions
{
    [Binding]
    public sealed class WashTradeSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private WashTradeRuleParameters _washTradeRuleParameters;
        private UniverseSelectionState _universeSelectionState;

        // wash trade factory and arguments
        private ICurrencyConverter _currencyConverter;
        private IWashTradePositionPairer _positionPairer;
        private IWashTradeClustering _washTradeClustering;
        private IUniverseOrderFilter _universeOrderFilter;
        private IUniverseMarketCacheFactory _universeMarketCacheFactory;
        private ILogger<WashTradeRule> _logger;
        private ILogger<TradingHistoryStack> _tradingLogger;
        private WashTradeRuleFactory _washTradeRuleFactory;

        // wash trade run
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        public WashTradeSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
        {
            _scenarioContext = scenarioContext;
            _universeSelectionState = universeSelectionState;

            _currencyConverter = A.Fake<ICurrencyConverter>();
            _positionPairer = A.Fake<IWashTradePositionPairer>();
            _washTradeClustering = A.Fake<IWashTradeClustering>();
            _universeOrderFilter = A.Fake<IUniverseOrderFilter>();
            _universeMarketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            _logger = new NullLogger<WashTradeRule>();
            _tradingLogger = new NullLogger<TradingHistoryStack>();

            _washTradeRuleFactory =
                new WashTradeRuleFactory(
                    _currencyConverter,
                    _positionPairer,
                    _washTradeClustering,
                    _universeOrderFilter,
                    _universeMarketCacheFactory,
                    _logger,
                    _tradingLogger);

            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
        }

        [Given(@"I have the wash trade rule average netting parameter values")]
        public void GivenIHaveTheWashTradeRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                _scenarioContext.Pending();
                return;
            }

            var parameters = ruleParameters.CreateInstance<WashTradeApiParameters>();

            _washTradeRuleParameters =
                new WashTradeRuleParameters(
                    "0",
                    new System.TimeSpan(parameters.WindowHours, 0, 0),
                    true,
                    false,
                    false,
                    parameters.MinimumNumberOfTrades,
                    parameters.MaximumPositionChangeValue,
                    parameters.MaximumAbsoluteValueChange,
                    parameters.MaximumAbsoluteValueChangeCurrency,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    new[] { ClientOrganisationalFactors.None },
                    true);
        }

        [When(@"I run the wash trade rule")]
        public void WhenIRunTheWashTradeRule()
        {
            var washTradeRule = 
                _washTradeRuleFactory.Build(
                    _washTradeRuleParameters,
                    _ruleCtx,
                    _alertStream,
                    Rules.RuleRunMode.ForceRun);

            foreach (var universeEvent in _universeSelectionState.SelectedUniverse.UniverseEvents)
                washTradeRule.OnNext(universeEvent);
        }

        [Then(@"I will have (.*) wash trade alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }
    }
}

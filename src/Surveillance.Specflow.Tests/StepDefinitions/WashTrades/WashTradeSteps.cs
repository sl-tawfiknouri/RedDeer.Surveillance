namespace Surveillance.Specflow.Tests.StepDefinitions.WashTrades
{
    using System;
    using System.Collections.Generic;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Currency;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.WashTrade;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public sealed class WashTradeSteps
    {
        private readonly ScenarioContext _scenarioContext;

        private readonly IUniverseAlertStream _alertStream;

        // wash trade factory and arguments
        private readonly ICurrencyConverterService _currencyConverterService;

        private readonly EquityRuleWashTradeFactory _equityRuleWashTradeFactory;

        private readonly ILogger<WashTradeRule> _logger;

        // wash trade run
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        private readonly ILogger<TradingHistoryStack> _tradingLogger;

        private readonly IUniverseMarketCacheFactory _universeMarketCacheFactory;

        private readonly IUniverseEquityOrderFilterService _universeOrderFilterService;

        private readonly UniverseSelectionState _universeSelectionState;

        private readonly IClusteringService _washTradeClustering;

        private WashTradeRuleEquitiesParameters _washTradeRuleEquitiesParameters;

        public WashTradeSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
        {
            this._scenarioContext = scenarioContext;
            this._universeSelectionState = universeSelectionState;

            var exchangeRateApiRepository = A.Fake<IExchangeRateApiCachingDecorator>();

            var exchangeRateDto = new ExchangeRateDto
                                      {
                                          DateTime = new DateTime(2018, 01, 01),
                                          Name = "GBX/USD",
                                          FixedCurrency = "GBX",
                                          VariableCurrency = "USD",
                                          Rate = 200d
                                      };

            A.CallTo(() => exchangeRateApiRepository.Get(A<DateTime>.Ignored, A<DateTime>.Ignored)).Returns(
                new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
                    {
                        { new DateTime(2018, 01, 01), new[] { exchangeRateDto } }
                    });

            var currencyLogger = new NullLogger<CurrencyConverterService>();
            this._currencyConverterService = new CurrencyConverterService(exchangeRateApiRepository, currencyLogger);

            this._washTradeClustering = new ClusteringService();
            this._universeOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            this._universeMarketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            this._logger = new NullLogger<WashTradeRule>();
            this._tradingLogger = new NullLogger<TradingHistoryStack>();

            this._equityRuleWashTradeFactory = new EquityRuleWashTradeFactory(
                this._currencyConverterService,
                this._washTradeClustering,
                this._universeOrderFilterService,
                this._universeMarketCacheFactory,
                this._logger,
                this._tradingLogger);

            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();
        }

        [Given(@"I have the wash trade rule parameter values")]
        public void GivenIHaveTheWashTradeRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                this._scenarioContext.Pending();
                return;
            }

            var parameters = ruleParameters.CreateInstance<WashTradeApiParameters>();

            this._washTradeRuleEquitiesParameters = new WashTradeRuleEquitiesParameters(
                "0",
                new TimeSpan(parameters.WindowHours, 0, 0),
                parameters.UseAverageNetting.GetValueOrDefault(false),
                parameters.UseClustering.GetValueOrDefault(false),
                parameters.MinimumNumberOfTrades,
                parameters.MaximumPositionChangeValue,
                parameters.MaximumAbsoluteValueChange,
                parameters.MaximumAbsoluteValueChangeCurrency,
                parameters.ClusteringPositionMinimumNumberOfTrades,
                parameters.ClusteringPercentageValueDifferenceThreshold,
                new[] { ClientOrganisationalFactors.None },
                true,
                true);
        }

        [Then(@"I will have (.*) wash trade alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }

        [When(@"I run the wash trade rule")]
        public void WhenIRunTheWashTradeRule()
        {
            var washTradeRule = this._equityRuleWashTradeFactory.Build(
                this._washTradeRuleEquitiesParameters,
                this._ruleCtx,
                this._alertStream,
                RuleRunMode.ForceRun);

            foreach (var universeEvent in this._universeSelectionState.SelectedUniverse.UniverseEvents)
                washTradeRule.OnNext(universeEvent);
        }
    }
}
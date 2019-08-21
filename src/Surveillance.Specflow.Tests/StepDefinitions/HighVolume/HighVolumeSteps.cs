namespace Surveillance.Specflow.Tests.StepDefinitions.HighProfit
{
    using System;
    using System.Collections.Generic;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora.BMLL;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Currency;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.HighVolume;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public sealed class HighVolumeSteps
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private readonly EquityRuleHighVolumeFactory _equityRuleHighVolumeFactory;

        private readonly IUniverseMarketCacheFactory _interdayUniverseMarketCacheFactory;

        private readonly ILogger<HighVolumeRule> _logger;

        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        private readonly ScenarioContext _scenarioContext;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private readonly ILogger<TradingHistoryStack> _tradingLogger;

        private readonly IUniverseEquityOrderFilterService _universeOrderFilterService;

        private readonly UniverseSelectionState _universeSelectionState;

        private ICurrencyConverterService _currencyConverterService;

        private HighVolumeRuleEquitiesParameters _highVolumeRuleEquitiesParameters;

        public HighVolumeSteps(ScenarioContext scenarioContext, UniverseSelectionState universeSelectionState)
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
                                          Rate = 0.02d
                                      };

            A.CallTo(() => exchangeRateApiRepository.Get(A<DateTime>.Ignored, A<DateTime>.Ignored)).Returns(
                new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
                    {
                        { new DateTime(2018, 01, 01), new[] { exchangeRateDto } }
                    });

            var repository = A.Fake<IMarketOpenCloseApiCachingDecorator>();

            A.CallTo(() => repository.Get()).Returns(
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

            this._interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());

            var currencyLogger = new NullLogger<CurrencyConverterService>();
            this._currencyConverterService = new CurrencyConverterService(exchangeRateApiRepository, currencyLogger);
            this._universeOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            this._logger = new NullLogger<HighVolumeRule>();
            this._tradingLogger = new NullLogger<TradingHistoryStack>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();
            this._dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            this._equityRuleHighVolumeFactory = new EquityRuleHighVolumeFactory(
                this._universeOrderFilterService,
                this._interdayUniverseMarketCacheFactory,
                this._tradingHoursService,
                this._logger,
                this._tradingLogger);
        }

        [Given(@"I have the high volume rule parameter values")]
        public void GivenIHaveTheHighVolumeRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                this._scenarioContext.Pending();
                return;
            }

            var parameters = ruleParameters.CreateInstance<HighVolumeApiParameters>();

            this._highVolumeRuleEquitiesParameters = new HighVolumeRuleEquitiesParameters(
                "0",
                TimeSpan.FromHours(parameters.WindowHours),
                parameters.HighVolumePercentageDaily,
                parameters.HighVolumePercentageWindow,
                parameters.HighVolumePercentageMarketCap,
                new[] { ClientOrganisationalFactors.None },
                true,
                true);
        }

        [Then(@"I will have (.*) high volume alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == alertCount);
        }

        [When(@"I run the high volume rule")]
        public void WhenIRunTheHighVolumeRule()
        {
            var highVolumeRule = this._equityRuleHighVolumeFactory.Build(
                this._highVolumeRuleEquitiesParameters,
                this._ruleCtx,
                this._alertStream,
                this._dataRequestSubscriber,
                RuleRunMode.ForceRun);

            foreach (var universeEvent in this._universeSelectionState.SelectedUniverse.UniverseEvents)
                highVolumeRule.OnNext(universeEvent);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Financial;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Currency;
using Surveillance.Currency.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
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

            var exchangeRateApiRepository = A.Fake<IExchangeRateApiCachingDecoratorRepository>();

            var exchangeRateDto = new ExchangeRateDto { DateTime = new DateTime(2018, 01, 01), Name = "GBX/USD", FixedCurrency = "GBX", VariableCurrency = "USD", Rate = 200d };

            A.CallTo(() =>
                    exchangeRateApiRepository.Get(A<DateTime>.Ignored, A<DateTime>.Ignored))
                .Returns(new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
                        {
                            { new DateTime(2018, 01, 01), new ExchangeRateDto[] { exchangeRateDto }}
                        });

            var currencyLogger = new NullLogger<CurrencyConverter>();
            _currencyConverter = new CurrencyConverter(exchangeRateApiRepository, currencyLogger);

            _positionPairer = new WashTradePositionPairer();
            _washTradeClustering = new WashTradeClustering();
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

        [Given(@"I have the wash trade rule parameter values")]
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
                    parameters.UseAverageNetting.GetValueOrDefault(false),
                    parameters.UsePairing.GetValueOrDefault(false),
                    parameters.UseClustering.GetValueOrDefault(false),
                    parameters.MinimumNumberOfTrades,
                    parameters.MaximumPositionChangeValue,
                    parameters.MaximumAbsoluteValueChange,
                    parameters.MaximumAbsoluteValueChangeCurrency,
                    parameters.PairingPositionMinimumNumberOfPairedTrades,
                    parameters.PairingPositionPercentagePriceChangeThresholdPerPair,
                    parameters.PairingPositionPercentageVolumeDifferenceThreshold,
                    parameters.PairingPositionMaximumAbsoluteCurrencyAmount,
                    parameters.PairingPositionMaximumAbsoluteCurrency,
                    parameters.ClusteringPositionMinimumNumberOfTrades,
                    parameters.ClusteringPercentageValueDifferenceThreshold,
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

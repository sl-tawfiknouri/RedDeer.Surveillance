﻿using System;
using System.Collections.Generic;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Currency;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.WashTrade;
using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Specflow.Tests.StepDefinitions.Universe;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Surveillance.Specflow.Tests.StepDefinitions.WashTrades
{
    [Binding]
    public sealed class WashTradeSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private WashTradeRuleEquitiesParameters _washTradeRuleEquitiesParameters;
        private UniverseSelectionState _universeSelectionState;

        // wash trade factory and arguments
        private ICurrencyConverterService _currencyConverterService;
        private IWashTradePositionPairer _positionPairer;
        private IClusteringService _washTradeClustering;
        private IUniverseEquityOrderFilterService _universeOrderFilterService;
        private IUniverseMarketCacheFactory _universeMarketCacheFactory;
        private ILogger<WashTradeRule> _logger;
        private ILogger<TradingHistoryStack> _tradingLogger;
        private EquityRuleWashTradeFactory _equityRuleWashTradeFactory;

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

            var currencyLogger = new NullLogger<CurrencyConverterService>();
            _currencyConverterService = new CurrencyConverterService(exchangeRateApiRepository, currencyLogger);

            _positionPairer = new WashTradePositionPairer();
            _washTradeClustering = new ClusteringService();
            _universeOrderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            _universeMarketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            _logger = new NullLogger<WashTradeRule>();
            _tradingLogger = new NullLogger<TradingHistoryStack>();

            _equityRuleWashTradeFactory =
                new EquityRuleWashTradeFactory(
                    _currencyConverterService,
                    _positionPairer,
                    _washTradeClustering,
                    _universeOrderFilterService,
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

            _washTradeRuleEquitiesParameters =
                new WashTradeRuleEquitiesParameters(
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
                    parameters.PairingPositionMaximumAbsoluteMoney,
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
                _equityRuleWashTradeFactory.Build(
                    _washTradeRuleEquitiesParameters,
                    _ruleCtx,
                    _alertStream,
                    RuleRunMode.ForceRun);

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

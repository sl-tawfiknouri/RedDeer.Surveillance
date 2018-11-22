﻿using System;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Currency.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.WashTrade;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories
{
    public class WashTradeRuleFactory : IWashTradeRuleFactory
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IWashTradePositionPairer _positionPairer;
        private readonly IWashTradeClustering _clustering;
        private readonly ILogger _logger;

        public static string Version { get; } = Versioner.Version(1, 0);

        public WashTradeRuleFactory(
            ICurrencyConverter currencyConverter,
            IWashTradePositionPairer positionPairer,
            IWashTradeClustering clustering,
            ILogger<WashTradeRule> logger)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _positionPairer = positionPairer ?? throw new ArgumentNullException(nameof(positionPairer));
            _clustering = clustering ?? throw new ArgumentNullException(nameof(clustering));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IWashTradeRule Build(IWashTradeRuleParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx, IUniverseAlertStream alertStream)
        {
            if (ruleCtx == null)
            {
                throw new ArgumentNullException(nameof(ruleCtx));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return new WashTradeRule(parameters, ruleCtx, _positionPairer, _clustering, alertStream, _currencyConverter, _logger);
        }
    }
}
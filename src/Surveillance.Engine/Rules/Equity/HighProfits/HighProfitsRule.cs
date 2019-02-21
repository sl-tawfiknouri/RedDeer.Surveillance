﻿using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

// ReSharper disable AssignNullToNotNullAttribute
namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    /// <summary>
    /// Calculate the profits from the trade
    /// </summary>
    public class HighProfitsRule : IHighProfitRule
    {
        private readonly IHighProfitStreamRule _streamRule;
        private readonly IHighProfitMarketClosureRule _marketClosureRule;
        private readonly ILogger<HighProfitsRule> _logger;

        public HighProfitsRule(
            IHighProfitStreamRule streamRule,
            IHighProfitMarketClosureRule marketClosureRule,
            ILogger<HighProfitsRule> logger)
        {
            _streamRule = streamRule ?? throw new ArgumentNullException(nameof(streamRule));
            _marketClosureRule = marketClosureRule ?? throw new ArgumentNullException(nameof(marketClosureRule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnCompleted()
        {
            _logger.LogInformation($"HighProfitsRule OnCompleted() event received. Passing onto high profit and high profit market close rules.");
            _streamRule.OnCompleted();
            _marketClosureRule.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"HighProfitsRule OnCompleted() event received", error);
            _streamRule.OnError(error);
            _marketClosureRule.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            _logger.LogInformation($"HighProfitsRule OnNext() event received at {value.EventTime}. Passing onto high profit and high profit market close rules.");

            // if removing the market closure rule
            // ensure that the alert subscriber is also updated to remove expectation of 2x flush events
            _streamRule.OnNext(value);
            _marketClosureRule.OnNext(value);
        }

        public Domain.Scheduling.Rules Rule { get; } = Domain.Scheduling.Rules.HighProfits;
        public string Version { get; } = EquityRuleHighProfitFactory.Version;

        public object Clone()
        {
            return new HighProfitsRule(
                (IHighProfitStreamRule)_streamRule.Clone(),
                (IHighProfitMarketClosureRule)_marketClosureRule.Clone(),
                _logger);
        }
    }
}